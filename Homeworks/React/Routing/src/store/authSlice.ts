import { createSlice, type PayloadAction } from '@reduxjs/toolkit';

// 1. Форма нашего состояния
interface AuthState {
  isAuthenticated: boolean;
  username: string | null;
}

// 2. Начальные данные (аналог useState(null))
const initialState: AuthState = {
  isAuthenticated: false,
  username: null,
};

export const authSlice = createSlice({
  name: 'auth', // префикс для type: 'auth/login', 'auth/logout'
  initialState,
  reducers: {
    // 3. Функции, описывающие, как меняется состояние
    login: (state, action: PayloadAction<string>) => {
      state.isAuthenticated = true;
      state.username = action.payload; // данные из dispatch(login("Ivan"))
    },
    logout: (state) => {
      state.isAuthenticated = false;
      state.username = null;
    },
  },
});

export const { login, logout } = authSlice.actions;
export default authSlice.reducer;