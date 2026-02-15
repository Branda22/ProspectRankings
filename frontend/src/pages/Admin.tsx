import { useState } from 'react';
import {
  Alert,
  Button,
  Container,
  Divider,
  Group,
  Paper,
  Stack,
  Text,
  TextInput,
  Textarea,
  Title,
} from '@mantine/core';
import { Navigate } from 'react-router-dom';
import { useAppSelector } from '../store';
import api from '../services/api';

export default function Admin() {
  const { isAuthenticated } = useAppSelector((state) => state.auth);

  const [source, setSource] = useState('');
  const [json, setJson] = useState('');
  const [uploading, setUploading] = useState(false);
  const [calculating, setCalculating] = useState(false);
  const [status, setStatus] = useState<{
    type: 'success' | 'error';
    message: string;
  } | null>(null);

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  const handleUpload = async () => {
    if (!source.trim()) {
      setStatus({ type: 'error', message: 'Source name is required' });
      return;
    }

    let list;
    try {
      list = JSON.parse(json);
      if (!Array.isArray(list)) {
        throw new Error('JSON must be an array');
      }
    } catch (e) {
      setStatus({
        type: 'error',
        message: `Invalid JSON: ${e instanceof Error ? e.message : 'parse error'}`,
      });
      return;
    }

    setUploading(true);
    setStatus(null);
    try {
      const res = await api.post('/prospects', { source: source.trim(), list });
      const count = Array.isArray(res.data) ? res.data.length : 0;
      setStatus({
        type: 'success',
        message: `Uploaded ${count} prospects from "${source.trim()}"`,
      });
      setJson('');
    } catch (e) {
      setStatus({
        type: 'error',
        message: `Upload failed: ${e instanceof Error ? e.message : 'unknown error'}`,
      });
    } finally {
      setUploading(false);
    }
  };

  const handleCalculate = async () => {
    setCalculating(true);
    setStatus(null);
    try {
      await api.post('/prospects/calculate');
      setStatus({
        type: 'success',
        message: 'Rankings calculated successfully',
      });
    } catch (e) {
      setStatus({
        type: 'error',
        message: `Calculation failed: ${e instanceof Error ? e.message : 'unknown error'}`,
      });
    } finally {
      setCalculating(false);
    }
  };

  return (
    <Container size="md" py="md">
      <Stack gap="lg">
        <div>
          <Title order={2}>Admin</Title>
          <Text c="dimmed" size="sm" mt={4}>
            Upload prospect lists and calculate rankings
          </Text>
        </div>

        {status && (
          <Alert
            color={status.type === 'success' ? 'green' : 'red'}
            variant="light"
            withCloseButton
            onClose={() => setStatus(null)}
          >
            {status.message}
          </Alert>
        )}

        <Paper p="md" radius="md" withBorder>
          <Stack gap="md">
            <Title order={4}>Upload Prospect List</Title>
            <TextInput
              label="Source"
              placeholder='e.g. "MLB Pipeline", "FanGraphs"'
              value={source}
              onChange={(e) => setSource(e.currentTarget.value)}
            />
            <Textarea
              label="Prospect JSON"
              placeholder='[{ "player_name": "...", "team": "...", "position": "...", "age": 21, "eta": "2026", "rank": 1 }]'
              minRows={10}
              autosize
              maxRows={20}
              value={json}
              onChange={(e) => setJson(e.currentTarget.value)}
              styles={{ input: { fontFamily: 'monospace', fontSize: 13 } }}
            />
            <Group>
              <Button onClick={handleUpload} loading={uploading}>
                Upload Prospects
              </Button>
            </Group>
          </Stack>
        </Paper>

        <Divider />

        <Paper p="md" radius="md" withBorder>
          <Stack gap="md">
            <Title order={4}>Calculate Rankings</Title>
            <Text size="sm" c="dimmed">
              Run the Rankle algorithm on all uploaded prospect data to generate
              aggregate rankings.
            </Text>
            <Group>
              <Button
                onClick={handleCalculate}
                loading={calculating}
                variant="gradient"
                gradient={{ from: 'yellow', to: 'orange', deg: 90 }}
              >
                Calculate Rankings
              </Button>
            </Group>
          </Stack>
        </Paper>
      </Stack>
    </Container>
  );
}
