using backend.Data.Repositories;
using backend.Models;

namespace backend.Services;

public interface IRankingService
{
    Task CalculateRankingAsync();
}

public class RankingService : IRankingService
{
    private const double ScaleMultiplier = 29.4;
    private const double ScoreCenter = 50.0;
    private const int ShrinkageK = 2;
    private const double ConsensusBand = 10.0;
    private const double TierCliffMultiplier = 2.5;
    private const int TierHalfWindow = 5;
    private const double TierZeroMultiplier = 2.0;
    private const int TierZeroMinGaps = 19;

    private readonly IProspectRepository _prospectRepository;
    private readonly IRankingRepository _rankingRepository;

    public RankingService(IProspectRepository prospectRepository, IRankingRepository rankingRepository)
    {
        _prospectRepository = prospectRepository;
        _rankingRepository = rankingRepository;
    }

    public async Task CalculateRankingAsync()
    {
        var allProspects = (await _prospectRepository.GetAllAsync(null)).ToList();
        if (allProspects.Count == 0)
            return;

        // Compute list_length per source (count of prospects with that source string)
        var listLengths = allProspects
            .GroupBy(p => p.Source ?? "", StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

        // Group prospects by player name (case-insensitive)
        var playerGroups = allProspects
            .GroupBy(p => p.PlayerName.Trim(), StringComparer.OrdinalIgnoreCase);

        var computed = new List<Ranking>();

        foreach (var group in playerGroups)
        {
            var entries = group.ToList();
            var ranking = ComputePlayerRanking(entries, listLengths);
            computed.Add(ranking);
        }

        // Sort by score descending, assign display rank
        computed = computed.OrderByDescending(r => r.Score).ToList();
        for (int i = 0; i < computed.Count; i++)
        {
            computed[i].Rank = i + 1;
        }

        // Compute tiers
        AssignTiers(computed);

        // Persist: truncate and bulk insert
        await _rankingRepository.DeleteAllAsync();
        await _rankingRepository.BulkCreateAsync(computed);
    }

    private static Ranking ComputePlayerRanking(List<Prospect> entries, Dictionary<string, int> listLengths)
    {
        // Resolve metadata from the highest-ranked (lowest rank number) entry
        var best = entries.OrderBy(e => e.Rank).First();

        int sourceCount = entries.Count;

        if (sourceCount == 0)
        {
            return new Ranking
            {
                PlayerName = best.PlayerName,
                Team = best.Team,
                Position = best.Position,
                Age = best.Age,
                ETA = best.ETA,
                Rank = 0,
                Score = 0,
                Volatility = "N/A",
                Consensus = 0,
                Median = null,
                Sd = null,
                Tier = 1,
                SourceCount = 0
            };
        }

        // Compute z-scores
        var zScores = new List<double>();
        var rawRanks = new List<int>();
        var percentiles = new List<double>();

        foreach (var entry in entries)
        {
            int listLength = listLengths.GetValueOrDefault(entry.Source ?? "", 2);
            if (listLength < 2) listLength = 2;

            double mean = (listLength + 1.0) / 2.0;
            double stddev = Math.Sqrt(((double)listLength * listLength - 1.0) / 12.0);

            double z = -(entry.Rank - mean) / stddev;
            zScores.Add(z);
            rawRanks.Add(entry.Rank);

            double percentile = 100.0 * (1.0 - (entry.Rank - 1.0) / (listLength - 1.0));
            percentiles.Add(percentile);
        }

        // Average z and raw score
        double zAvg = zScores.Average();
        double rawScore = ScoreCenter + zAvg * ScaleMultiplier;

        // Bayesian shrinkage for single-source
        if (sourceCount == 1)
        {
            rawScore = (rawScore + ShrinkageK * ScoreCenter) / (1 + ShrinkageK);
        }

        // Clamp to [0, 100] and round to one decimal
        double rankleScore = Math.Round(Math.Clamp(rawScore, 0.0, 100.0), 1);

        // Median of raw ranks
        double median = ComputeMedian(rawRanks.Select(r => (double)r).ToList());

        // Volatility (sample stddev of z-scores)
        double? sd = null;
        string volatility = "N/A";
        if (sourceCount >= 2)
        {
            double variance = zScores.Select(z => (z - zAvg) * (z - zAvg)).Sum() / (sourceCount - 1);
            sd = Math.Sqrt(variance);

            volatility = sd.Value switch
            {
                < 0.3 => "Low",
                < 0.7 => "Moderate",
                < 0.85 => "High",
                _ => "Extreme"
            };
        }

        // Consensus agreement
        double medianPercentile = ComputeMedian(percentiles);
        int consensus = percentiles.Count(p => Math.Abs(p - medianPercentile) <= ConsensusBand);

        return new Ranking
        {
            PlayerName = best.PlayerName,
            Team = best.Team,
            Position = best.Position,
            Age = best.Age,
            ETA = best.ETA,
            Rank = 0, // assigned after sorting
            Score = rankleScore,
            Volatility = volatility,
            Consensus = consensus,
            Median = median,
            Sd = sd.HasValue ? Math.Round(sd.Value, 2) : null,
            Tier = 1, // assigned after tier detection
            SourceCount = sourceCount
        };
    }

    private static double ComputeMedian(List<double> values)
    {
        if (values.Count == 0) return 0;
        var sorted = values.OrderBy(v => v).ToList();
        int mid = sorted.Count / 2;
        if (sorted.Count % 2 == 0)
            return (sorted[mid - 1] + sorted[mid]) / 2.0;
        return sorted[mid];
    }

    private static void AssignTiers(List<Ranking> sorted)
    {
        if (sorted.Count <= 1)
        {
            if (sorted.Count == 1)
                sorted[0].Tier = 1;
            return;
        }

        // Compute gaps
        var gaps = new double[sorted.Count - 1];
        for (int i = 0; i < gaps.Length; i++)
        {
            gaps[i] = sorted[i].Score - sorted[i + 1].Score;
        }

        // Find tier breaks
        var tierBreaks = new List<int>();
        for (int i = 0; i < gaps.Length; i++)
        {
            int start = Math.Max(0, i - TierHalfWindow);
            int end = Math.Min(gaps.Length - 1, i + TierHalfWindow);

            double localMean = 0;
            for (int j = start; j <= end; j++)
                localMean += gaps[j];
            localMean /= (end - start + 1);

            if (localMean > 0 && gaps[i] > TierCliffMultiplier * localMean)
            {
                tierBreaks.Add(i);
            }
        }

        // Tier 0 check (elite #1)
        bool hasTierZero = false;
        if (gaps.Length >= TierZeroMinGaps)
        {
            double avgTop20 = 0;
            for (int i = 0; i < TierZeroMinGaps; i++)
                avgTop20 += gaps[i];
            avgTop20 /= TierZeroMinGaps;

            if (gaps[0] > TierZeroMultiplier * avgTop20)
                hasTierZero = true;
        }

        // Assign tiers
        for (int i = 0; i < sorted.Count; i++)
        {
            if (hasTierZero && i == 0)
            {
                sorted[i].Tier = 0;
            }
            else
            {
                int tier = 1 + tierBreaks.Count(b => b < i);
                sorted[i].Tier = tier;
            }
        }
    }
}
