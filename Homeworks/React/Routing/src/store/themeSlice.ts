// src/store/themeSlice.ts
import { createSlice, type PayloadAction } from '@reduxjs/toolkit';

// 1. Выносим тип в одно место
export type ThemeMode = 'light' | 'dark';

// 2. Используем его в интерфейсе
interface ThemeState {
  mode: ThemeMode;
}

const initialState: ThemeState = {
  mode: 'dark',
};

export const themeSlice = createSlice({
  name: 'theme',
  initialState,
  reducers: {
    // 3. Используем тот же тип для payload
    setMode: (state, action: PayloadAction<ThemeMode>) => {
      state.mode = action.payload;
    },
    toggleMode: (state) => {
      state.mode = state.mode === 'light' ? 'dark' : 'light';
    },
  },
});

export const { setMode, toggleMode } = themeSlice.actions;
export default themeSlice.reducer;