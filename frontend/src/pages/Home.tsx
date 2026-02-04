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
import { fetchProspects, fetchSources } from '../store/prospectsSlice';

function getRankBadgeColor(rank: number) {
  if (rank <= 3) return 'yellow';
  if (rank <= 6) return 'gray';
  if (rank <= 10) return 'orange';
  return 'dark';
}

const POSITIONS = [
  'C', '1B', '2B', '3B', 'SS', 'OF',
  'LHP', 'RHP', 'BHP',
];

export default function Home() {
  const dispatch = useAppDispatch();
  const { prospects, sources, isLoading } = useAppSelector(
    (state) => state.prospects
  );

  const [search, setSearch] = useState('');
  const [positionFilter, setPositionFilter] = useState<string | null>(null);
  const [teamFilter, setTeamFilter] = useState<string | null>(null);
  const [sourceFilter, setSourceFilter] = useState<string | null>(null);

  useEffect(() => {
    dispatch(fetchSources());
  }, [dispatch]);

  useEffect(() => {
    const sourceId = sourceFilter ? parseInt(sourceFilter) : undefined;
    dispatch(fetchProspects(sourceId));
  }, [dispatch, sourceFilter]);

  const teams = useMemo(() => {
    const unique = [...new Set(prospects.map((p) => p.team))].sort();
    return unique.map((t) => ({ value: t, label: t }));
  }, [prospects]);

  const sourceOptions = useMemo(
    () => sources.map((s) => ({ value: String(s.id), label: s.name })),
    [sources]
  );

  const filtered = useMemo(() => {
    return prospects.filter((p) => {
      if (search && !p.playerName.toLowerCase().includes(search.toLowerCase())) {
        return false;
      }
      if (positionFilter && p.position !== positionFilter) return false;
      if (teamFilter && p.team !== teamFilter) return false;
      return true;
    });
  }, [prospects, search, positionFilter, teamFilter]);

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
            <Select
              placeholder="Source"
              data={sourceOptions}
              value={sourceFilter}
              onChange={setSourceFilter}
              clearable
              style={{ minWidth: 180 }}
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
          <Table.ScrollContainer minWidth={600}>
            <Table striped highlightOnHover>
              <Table.Thead>
                <Table.Tr>
                  <Table.Th style={{ width: 60 }}>#</Table.Th>
                  <Table.Th>Player</Table.Th>
                  <Table.Th>Pos</Table.Th>
                  <Table.Th>Team</Table.Th>
                  <Table.Th>Age</Table.Th>
                  <Table.Th>ETA</Table.Th>
                  <Table.Th>Source</Table.Th>
                </Table.Tr>
              </Table.Thead>
              <Table.Tbody>
                {filtered.map((p) => (
                  <Table.Tr key={p.id}>
                    <Table.Td>
                      <Badge
                        color={getRankBadgeColor(p.rank)}
                        variant="filled"
                        size="sm"
                        radius="sm"
                        w={36}
                      >
                        {p.rank}
                      </Badge>
                    </Table.Td>
                    <Table.Td>
                      <Text fw={500} size="sm">
                        {p.playerName}
                      </Text>
                    </Table.Td>
                    <Table.Td>
                      <Badge variant="light" color="blue" size="sm">
                        {p.position}
                      </Badge>
                    </Table.Td>
                    <Table.Td>
                      <Text size="sm">{p.team}</Text>
                    </Table.Td>
                    <Table.Td>
                      <Text size="sm" c="dimmed">
                        {p.age || '—'}
                      </Text>
                    </Table.Td>
                    <Table.Td>
                      <Text size="sm" c="dimmed">
                        {p.eta || '—'}
                      </Text>
                    </Table.Td>
                    <Table.Td>
                      <Text size="sm" c="dimmed">
                        {p.source?.name || '—'}
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
