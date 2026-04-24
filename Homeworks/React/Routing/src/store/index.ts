import { configureStore } from '@reduxjs/toolkit';
import authReducer from './authSlice';
import themeReducer from './themeSlice';

// Собираем все редьюсеры в одно дерево
export const store = configureStore({
  reducer: {
    auth: authReducer, // ключ "auth" станет доступен как state.auth
    theme: themeReducer,
  },
});

// Выводим типы для хуков
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
