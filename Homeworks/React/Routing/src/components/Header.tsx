// src/components/Header.tsx
import { AppBar, Toolbar, Typography, Button, IconButton, Box } from '@mui/material';
import { Link } from 'react-router-dom';
import { useAppSelector, useAppDispatch } from '../hooks/redux';
import { logout } from '../store/authSlice';
import { toggleMode } from '../store/themeSlice';
import Brightness4Icon from '@mui/icons-material/Brightness4';
import Brightness7Icon from '@mui/icons-material/Brightness7';

export default function Header() {
  const { isAuthenticated, username } = useAppSelector((state) => state.auth);
  const mode = useAppSelector((state) => state.theme.mode);
  const dispatch = useAppDispatch();

  return (
    // 🟦 AppBar: верхняя панель приложения
    <AppBar position="static" color="transparent" elevation={1}>
      <Toolbar>
        {/* Логотип/Название */}
        <Typography variant="h6" component={Link} to="/" sx={{ flexGrow: 1, textDecoration: 'none', color: 'inherit' }}>
          MyApp
        </Typography>

        {/* 🟦 Box: контейнер для выравнивания кнопок */}
        <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
          {!isAuthenticated ? (
            <>
              <Button component={Link} to="/login" color="inherit">Вход</Button>
              <Button component={Link} to="/register" variant="outlined">Регистрация</Button>
            </>
          ) : (
            <>
              <Button component={Link} to="/profile" color="inherit">Профиль</Button>
              <Button component={Link} to="/settings" color="inherit">Настройки</Button>
              <Typography variant="body2" sx={{ mx: 1 }}>Привет, {username}</Typography>
              <Button onClick={() => dispatch(logout())} color="error" size="small">Выйти</Button>
            </>
          )}

          {/* 🟦 IconButton: кнопка-переключатель темы */}
          <IconButton onClick={() => dispatch(toggleMode())} color="inherit">
            {mode === 'dark' ? <Brightness7Icon /> : <Brightness4Icon />}
          </IconButton>
        </Box>
      </Toolbar>
    </AppBar>
  );
}