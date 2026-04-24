// src/pages/LoginPage.tsx
import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAppDispatch } from '../hooks/redux';
import { login } from '../store/authSlice';
import { type LoginPayload, mockLoginApi } from '../api/auth.mock';
import { Container, Box, Typography, TextField, Button, Alert, Paper } from '@mui/material';

export default function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    const payload: LoginPayload = { username, password };

    try {
      await mockLoginApi(payload);
      dispatch(login(username));
      navigate('/');
    } catch (err) {
      const message = (err as { message?: string }).message || 'Ошибка входа';
      setError(message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleGuestLogin = () => {
    dispatch(login('Гость'));
    navigate('/');
  };

  return (
    // 🟦 Container: центрирует контент по ширине
    <Container maxWidth="xs">
      {/* 🟦 Paper: карточка с фоном из темы (paper) и тенями */}
      <Paper elevation={3} sx={{ p: 4, mt: 8, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
        <Typography component="h1" variant="h5" gutterBottom>
          Вход в систему
        </Typography>

        {error && <Alert severity="error" sx={{ width: '100%', mb: 2 }}>{error}</Alert>}

        {/* 🟦 Box: обёртка формы */}
        <Box component="form" onSubmit={handleSubmit} sx={{ width: '100%' }}>
          {/* 🟦 TextField: умное поле ввода (лейбл, валидация, стилизация) */}
          <TextField
            fullWidth
            label="Логин"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            disabled={isLoading}
          />
          <TextField
            fullWidth
            label="Пароль"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            disabled={isLoading}
          />

          {/* 🟦 Button: кнопка с variant="contained" (залитая) */}
          <Button type="submit" fullWidth variant="contained" disabled={isLoading} sx={{ mt: 2, mb: 1 }}>
            {isLoading ? 'Вход...' : 'Войти'}
          </Button>

          <Button fullWidth variant="text" onClick={handleGuestLogin} disabled={isLoading}>
            Войти как Гость
          </Button>

          <Box sx={{ mt: 2, textAlign: 'center' }}>
            <Typography variant="body2">
              Нет аккаунта? <Link to="/register" style={{ textDecoration: 'none' }}>Зарегистрироваться</Link>
            </Typography>
          </Box>
        </Box>
      </Paper>
    </Container>
  );
}