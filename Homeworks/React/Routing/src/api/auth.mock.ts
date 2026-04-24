// src/api/auth.mock.ts

// 1. Контракт запроса
export interface LoginPayload {
  username: string;
  password: string;
}

// Только полезная нагрузка. В реальном проекте здесь будет токен, id, роль и т.д.
export interface AuthResponse {
  username: string;
  // accessToken?: string;
}

// 3. Тип ошибки
export interface ApiError {
  message: string;
}

// 4. Заглушка бэкенда
export const mockLoginApi = async (
  payload: LoginPayload
): Promise<AuthResponse> => {
  return new Promise((resolve, reject) => {
    // Имитация сетевой задержки (1 секунда)
    setTimeout(() => {
      // Простая валидация на стороне "сервера"
      if (payload.username.length >= 3 && payload.password === '1234') {
        resolve({ username: payload.username });
      } else {
        reject({
          message: 'Неверный логин или пароль. Попробуйте пароль: 1234',
        } as ApiError);
      }
    }, 1000);
  });
};

//регистрация
export interface RegisterPayload {
  username: string;
  email: string;
  password: string;
}

export const mockRegisterApi = async (
  payload: RegisterPayload
): Promise<AuthResponse> => {
  return new Promise((resolve, reject) => {
    setTimeout(() => {
      // Простая валидация на стороне "сервера"
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      
      if (!emailRegex.test(payload.email)) {
        return reject({ message: 'Некорректный формат email' });
      }
      if (payload.password.length < 6) {
        return reject({ message: 'Пароль должен содержать минимум 6 символов' });
      }
      if (payload.username.toLowerCase() === 'admin') {
        return reject({ message: 'Имя пользователя занято' });
      }

      // Успех: возвращаем имя для авто-входа
      resolve({ username: payload.username });
    }, 1000);
  });
};