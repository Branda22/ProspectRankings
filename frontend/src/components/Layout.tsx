import { AppShell, Group, Text, Button, Box } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store';
import { logout } from '../store/authSlice';

interface LayoutProps {
  children: React.ReactNode;
}

export default function Layout({ children }: LayoutProps) {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { isAuthenticated, user } = useAppSelector((state) => state.auth);

  const handleLogout = () => {
    dispatch(logout());
    navigate('/');
  };

  return (
    <AppShell header={{ height: 60 }} padding="md">
      <AppShell.Header
        style={{
          borderBottom: 'none',
          borderTop: '3px solid var(--mantine-color-blue-6)',
        }}
      >
        <Group h="100%" px="md" justify="space-between">
          <Group gap="xs">
            <Text
              size="xl"
              fw={800}
              variant="gradient"
              gradient={{ from: 'yellow', to: 'orange', deg: 90 }}
              style={{ letterSpacing: '0.15em', cursor: 'pointer' }}
              onClick={() => navigate('/')}
            >
              RANKLE
            </Text>
            <Box visibleFrom="sm">
              <Text size="sm" c="dimmed" ml="md">
                MLB Prospect Rankings
              </Text>
            </Box>
          </Group>
          <Group>
            {isAuthenticated ? (
              <>
                <Text size="sm" c="dimmed">
                  {user?.firstName || user?.email}
                </Text>
                <Button variant="subtle" size="xs" onClick={handleLogout}>
                  Logout
                </Button>
              </>
            ) : (
              <Button
                variant="light"
                size="xs"
                onClick={() => navigate('/login')}
              >
                Login
              </Button>
            )}
          </Group>
        </Group>
      </AppShell.Header>

      <AppShell.Main>{children}</AppShell.Main>
    </AppShell>
  );
}
