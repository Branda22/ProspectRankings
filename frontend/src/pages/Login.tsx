import { useState } from 'react';
import {
  TextInput,
  PasswordInput,
  Button,
  Paper,
  Title,
  Container,
  Stack,
  Anchor,
  Text,
  Alert,
} from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store';
import { login, register, clearError } from '../store/authSlice';

export default function Login() {
  const [isRegister, setIsRegister] = useState(false);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');

  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { isLoading, error } = useAppSelector((state) => state.auth);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const action = isRegister
      ? register({ email, password, firstName, lastName })
      : login({ email, password });

    const result = await dispatch(action);

    if (result.meta.requestStatus === 'fulfilled') {
      navigate('/');
    }
  };

  const toggleMode = () => {
    setIsRegister(!isRegister);
    dispatch(clearError());
  };

  return (
    <Container size={420} my={40}>
      <Title ta="center">
        {isRegister ? 'Create an account' : 'Welcome back'}
      </Title>
      <Text c="dimmed" size="sm" ta="center" mt={5}>
        {isRegister ? 'Already have an account? ' : "Don't have an account? "}
        <Anchor size="sm" component="button" onClick={toggleMode}>
          {isRegister ? 'Sign in' : 'Create one'}
        </Anchor>
      </Text>

      <Paper withBorder shadow="md" p={30} mt={30} radius="md">
        <form onSubmit={handleSubmit}>
          <Stack>
            {error && (
              <Alert color="red" variant="light">
                {error}
              </Alert>
            )}

            {isRegister && (
              <>
                <TextInput
                  label="First Name"
                  placeholder="John"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                />
                <TextInput
                  label="Last Name"
                  placeholder="Doe"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                />
              </>
            )}

            <TextInput
              label="Email"
              placeholder="you@example.com"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />

            <PasswordInput
              label="Password"
              placeholder="Your password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />

            <Button type="submit" fullWidth loading={isLoading}>
              {isRegister ? 'Register' : 'Sign in'}
            </Button>
          </Stack>
        </form>
      </Paper>
    </Container>
  );
}
