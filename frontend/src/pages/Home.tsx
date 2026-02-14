import { useEffect, useMemo, useState } from 'react';
import {
  Badge,
  Container,
  Group,
  Loader,
  Select,
  Stack,
  Table,
  Text,
  TextInput,
  Title,
  Center,
  Paper,
} from '@mantine/core';
import { useAppDispatch, useAppSelector } from '../store';
import { fetchRankings } from '../store/prospectsSlice';

function getRankBadgeColor(rank: number) {
  if (rank <= 3) return 'yellow';
  if (rank <= 6) return 'gray';
  if (rank <= 10) return 'orange';
  return 'dark';
}

function getVolatilityColor(volatility: string) {
  switch (volatility.toLowerCase()) {
    case 'low':
      return 'green';
    case 'moderate':
      return 'yellow';
    case 'high':
      return 'orange';
    case 'extreme':
      return 'red';
    default:
      return 'gray';
  }
}

function getTierColor(tier: number) {
  if (tier === 0) return 'yellow';
  if (tier === 1) return 'teal';
  if (tier === 2) return 'blue';
  if (tier === 3) return 'indigo';
  return 'gray';
}

const POSITIONS = [
  'C', '1B', '2B', '3B', 'SS', 'OF',
  'LHP', 'RHP', 'BHP',
];

export default function Home() {
  const dispatch = useAppDispatch();
  const { rankings, isLoading } = useAppSelector(
    (state) => state.prospects
  );

  const [search, setSearch] = useState('');
  const [positionFilter, setPositionFilter] = useState<string | null>(null);
  const [teamFilter, setTeamFilter] = useState<string | null>(null);

  useEffect(() => {
    dispatch(fetchRankings());
  }, [dispatch]);

  const teams = useMemo(() => {
    const unique = [...new Set(rankings.map((r) => r.team))].sort();
    return unique.map((t) => ({ value: t, label: t }));
  }, [rankings]);

  const filtered = useMemo(() => {
    return rankings.filter((r) => {
      if (search && !r.playerName.toLowerCase().includes(search.toLowerCase())) {
        return false;
      }
      if (positionFilter && r.position !== positionFilter) return false;
      if (teamFilter && r.team !== teamFilter) return false;
      return true;
    });
  }, [rankings, search, positionFilter, teamFilter]);

  return (
    <Container size="lg" py="md">
      <Stack gap="lg">
        <div>
          <Title order={2}>
            <Text
              component="span"
              inherit
              variant="gradient"
              gradient={{ from: 'yellow', to: 'orange', deg: 90 }}
            >
              Prospect Rankings
            </Text>
          </Title>
          <Text c="dimmed" size="sm" mt={4}>
            Aggregate prospect rankings from multiple sources
          </Text>
        </div>

        <Paper p="md" radius="md" withBorder>
          <Group grow preventGrowOverflow={false} wrap="wrap" gap="sm">
            <TextInput
              placeholder="Search players..."
              value={search}
              onChange={(e) => setSearch(e.currentTarget.value)}
              style={{ minWidth: 200 }}
            />
            <Select
              placeholder="Position"
              data={POSITIONS}
              value={positionFilter}
              onChange={setPositionFilter}
              clearable
              style={{ minWidth: 130 }}
            />
            <Select
              placeholder="Team"
              data={teams}
              value={teamFilter}
              onChange={setTeamFilter}
              clearable
              searchable
              style={{ minWidth: 130 }}
            />
          </Group>
        </Paper>

        {isLoading ? (
          <Center py="xl">
            <Loader size="lg" />
          </Center>
        ) : filtered.length === 0 ? (
          <Center py="xl">
            <Text c="dimmed">No prospects found</Text>
          </Center>
        ) : (
          <Table.ScrollContainer minWidth={800}>
            <Table striped highlightOnHover>
              <Table.Thead>
                <Table.Tr>
                  <Table.Th style={{ width: 60 }}>#</Table.Th>
                  <Table.Th>Player</Table.Th>
                  <Table.Th>Pos</Table.Th>
                  <Table.Th>Team</Table.Th>
                  <Table.Th>Score</Table.Th>
                  <Table.Th>Tier</Table.Th>
                  <Table.Th>Volatility</Table.Th>
                  <Table.Th>Consensus</Table.Th>
                  <Table.Th>Age</Table.Th>
                  <Table.Th>ETA</Table.Th>
                </Table.Tr>
              </Table.Thead>
              <Table.Tbody>
                {filtered.map((r) => (
                  <Table.Tr key={r.id}>
                    <Table.Td>
                      <Badge
                        color={getRankBadgeColor(r.rank)}
                        variant="filled"
                        size="sm"
                        radius="sm"
                        w={36}
                      >
                        {r.rank}
                      </Badge>
                    </Table.Td>
                    <Table.Td>
                      <Text fw={500} size="sm">
                        {r.playerName}
                      </Text>
                    </Table.Td>
                    <Table.Td>
                      <Badge variant="light" color="blue" size="sm">
                        {r.position}
                      </Badge>
                    </Table.Td>
                    <Table.Td>
                      <Text size="sm">{r.team}</Text>
                    </Table.Td>
                    <Table.Td>
                      <Text size="sm" fw={600}>
                        {r.score.toFixed(1)}
                      </Text>
                    </Table.Td>
                    <Table.Td>
                      <Badge
                        variant="light"
                        color={getTierColor(r.tier)}
                        size="sm"
                      >
                        {r.tier === 0 ? 'Elite' : `Tier ${r.tier}`}
                      </Badge>
                    </Table.Td>
                    <Table.Td>
                      <Badge
                        variant="light"
                        color={getVolatilityColor(r.volatility)}
                        size="sm"
                      >
                        {r.volatility || '—'}
                      </Badge>
                    </Table.Td>
                    <Table.Td>
                      <Text size="sm" c="dimmed">
                        {r.consensus}
                      </Text>
                    </Table.Td>
                    <Table.Td>
                      <Text size="sm" c="dimmed">
                        {r.age || '—'}
                      </Text>
                    </Table.Td>
                    <Table.Td>
                      <Text size="sm" c="dimmed">
                        {r.eta || '—'}
                      </Text>
                    </Table.Td>
                  </Table.Tr>
                ))}
              </Table.Tbody>
            </Table>
          </Table.ScrollContainer>
        )}

        <Text size="xs" c="dimmed" ta="center" py="md">
          Data sourced from multiple prospect ranking publications
        </Text>
      </Stack>
    </Container>
  );
}
