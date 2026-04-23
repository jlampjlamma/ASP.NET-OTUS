import axios from 'axios';

export const formatAxiosError = (error: unknown): string => {
  // 1. Проверяем, что это именно ошибка от axios
  if (axios.isAxiosError(error)) {
    // ✅ Сервер ответил, но со статусом не 2xx (404, 500 и т.д.)
    if (error.response) {
      // Часто бэкенд возвращает { message: "..." } в теле ошибки
      const serverMessage = error.response.data?.message || error.response.data?.error;
      return serverMessage || `Ошибка сервера: ${error.response.status} ${error.response.statusText}`;
    }
    
    // 🌐 Запрос ушёл, но ответа нет (нет интернета, CORS, таймаут, сервер упал)
    if (error.request) {
      return 'Нет ответа от сервера. Проверьте подключение к интернету.';
    }
    
    // ⚙️ Ошибка при настройке запроса (невалидный URL, неверные заголовки)
    return `Ошибка конфигурации запроса: ${error.message}`;
  }

  // 🆘 Не-Axios ошибка (например, синтаксическая или брошенная вручную)
  return error instanceof Error ? error.message : 'Произошла неизвестная ошибка';
};