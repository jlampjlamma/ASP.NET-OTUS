// src/components/ThemeWrapper.tsx
import { ThemeProvider as MuiThemeProvider, CssBaseline } from '@mui/material';
import { useAppSelector } from '../hooks/redux';
import { getMuiTheme } from '../theme/muiTheme';

/**
 * Обёртка, которая синхронизирует Redux-тему с MUI.
 * CssBaseline сбрасывает стандартные стили браузера и применяет фон из темы.
 */
export default function ThemeWrapper({ children }: { children: React.ReactNode }) {
  // Читаем текущий режим из Redux
  const mode = useAppSelector((state) => state.theme.mode);
  
  // Создаём тему "на лету" при каждом изменении режима
  const theme = getMuiTheme(mode);

  return (
    // MuiThemeProvider раздаёт тему всем дочерним компонентам MUI
    <MuiThemeProvider theme={theme}>
      {/* CssBaseline применяет глобальные стили (фон, шрифты, сброс отступов) */}
      <CssBaseline />
      {children}
    </MuiThemeProvider>
  );
}