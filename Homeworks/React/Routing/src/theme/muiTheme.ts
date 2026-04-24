// src/theme/muiTheme.ts
import { createTheme } from '@mui/material/styles';

/**
 * Фабрика темы MUI.
 * Принимает режим ('light' | 'dark') из Redux и возвращает готовый объект темы.
 */
export const getMuiTheme = (mode: 'light' | 'dark') => {
  return createTheme({
    // Базовый режим: переключает внутренние палитры MUI
    palette: {
      mode,
      
      // 🎨 Кастомные цвета по твоему запросу
      background: {
        // Фон всего приложения
        default: mode === 'light' ? '#F5F5DC' : '#2A0A0A', // Бежевый / Тёмно-бордовый
        // Фон карточек, панелей, форм
        paper: mode === 'light' ? '#FFF8E7' : '#3B0F0F',   // Светло-бежевый / Бордовый потемнее
      },
      
      // Основной цвет кнопок и акцентов
      primary: {
        main: mode === 'light' ? '#8B5A2B' : '#D4A5A5', // Коричневый / Пыльная роза (для контраста)
      },
      
      // Цвета текста
      text: {
        primary: mode === 'light' ? '#2C2C2C' : '#F0E6E6',
        secondary: mode === 'light' ? '#666666' : '#B8A9A9',
      },
    },

    // Типографика и скругления
    typography: {
      fontFamily: '"Segoe UI", Roboto, Helvetica, sans-serif',
    },
    shape: {
      borderRadius: 8, // Скругление углов у всех компонентов MUI
    },

    // Точечные правки стилей компонентов
    components: {
      MuiButton: {
        styleOverrides: {
          root: { textTransform: 'none', fontWeight: 600 }, // Убираем КАПС, делаем жирнее
        },
      },
      MuiTextField: {
        styleOverrides: {
          root: { marginBottom: 16 }, // Отступ снизу у полей ввода
        },
      },
    },
  });
};