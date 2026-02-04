import { Title, Text, Paper, Stack } from '@mantine/core';
import { useAppSelector } from '../store';

export default function Home() {
  const { user } = useAppSelector((state) => state.auth);

  return (
    <Stack gap="lg">
      <Title order={1}>Welcome to ProspectRankings</Title>

      <Paper shadow="xs" p="xl" withBorder>
        <Text size="lg">
          Hello, {user?.firstName || user?.email || 'User'}!
        </Text>
        <Text c="dimmed" mt="sm">
          This is your dashboard. Start building your prospect rankings here.
        </Text>
      </Paper>
    </Stack>
  );
}
