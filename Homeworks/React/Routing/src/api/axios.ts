// src/api/axios.ts

import axios, { AxiosError } from 'axios';

/**
 * Создаём экземпляр (инстанс) Axios с базовыми настройками.
 * Вместо того чтобы писать baseURL и заголовки в каждом запросе,
 * мы выносим их сюда. Все запросы через этот инстанс наследуют настройки.
 */
const apiClient = axios.create({
  // Базовый URL, который будет добавляться ко всем относительным путям
  baseURL: 'https://jsonplaceholder.typicode.com',
  
  // Таймаут: если сервер не ответил за 5 секунд, запрос прервётся
  timeout: 5000,
  
  // Заголовки по умолчанию
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
});

// ==========================================
// 🛡 ИНТЕРЦЕПТОР ЗАПРОСОВ (Request Interceptor)
// ==========================================
// Срабатывает ПЕРЕД отправкой каждого запроса.
// Идеальное место для добавления токенов авторизации, логирования или изменения заголовков.
apiClient.interceptors.request.use(
  (config) => {
    // 📝 ПРИМЕР: Добавляем токен авторизации
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// ==========================================
// 🛡 ИНТЕРЦЕПТОР ОТВЕТОВ (Response Interceptor)
// ==========================================
// Срабатывает ПОСЛЕ получения ответа от сервера.
// Идеальное место для глобальной обработки ошибок (401, 500) или трансформации данных.
apiClient.interceptors.response.use(
  (response) => response, // Успех: просто пропускаем
  
  (error: AxiosError) => {
    // 🔍 1. Глобальная обработка специфичных статусов
    
    // 401: Сессия истекла или неверный токен
    if (error.response?.status === 401) {
      console.warn('🔒 Axios: 401 Unauthorized. Пользователь должен войти снова.');
      // В реальном проекте здесь был бы редирект:
      // navigate('/login'); 
      // localStorage.removeItem('accessToken');
    }
    
    // 403: Нет прав доступа
    if (error.response?.status === 403) {
      console.warn('🚫 Axios: 403 Forbidden. Доступ запрещён.');
    }
    
    // 404: Ресурс не найден
    if (error.response?.status === 404) {
      console.warn('🔍 Axios: 404 Not Found.');
    }
    
    // 500: Ошибка сервера
    if (error.response?.status === 500) {
      console.error('🔥 Axios: 500 Internal Server Error.');
    }

    // 🔍 2. Логируем детали ошибки для разработчика (в консоль)
    if (error.response) {
      // Сервер ответил со статусом не 2xx
      console.error('❌ Axios Error Details:', {
        status: error.response.status,
        data: error.response.data,
        url: error.config?.url,
      });
    } else if (error.request) {
      // Запрос ушёл, но ответа нет (сеть упала, CORS, таймаут)
      console.error('❌ Axios Network Error:', error.message);
    } else {
      // Ошибка при настройке запроса
      console.error('❌ Axios Config Error:', error.message);
    }

    // 🔍 3. Пробрасываем ошибку дальше в компонент
    // Компонент сам решит, показать ли уведомление пользователю
    return Promise.reject(error);
  }
);

export default apiClient;