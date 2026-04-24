import axios from 'axios';

/**
 * Превращает ошибку Axios в человекочитаемое сообщение.
 * 
 * @param error - Любая ошибка (unknown), пойманная в catch
 * @returns Строка с описанием проблемы для показа пользователю
 */
export const formatAxiosError = (error: unknown): string => {
  // 1. Проверяем, что это именно ошибка от axios
  if (axios.isAxiosError(error)) {
    
    // ✅ Сценарий А: Сервер ответил, но со статусом не 2xx (404, 500 и т.д.)
    if (error.response) {
      // Пытаемся взять сообщение от бэкенда (частый формат: { message: "..." })
      const backendData = error.response.data as { message?: string; error?: string };
      const serverMessage = backendData?.message || backendData?.error;
      
      // Если бэкенд дал сообщение — возвращаем его, иначе — стандартный текст
      return serverMessage || `Ошибка сервера: ${error.response.status} ${error.response.statusText}`;
    }
    
    // 🌐 Сценарий Б: Запрос ушёл, но ответа нет (нет интернета, CORS, таймаут)
    if (error.request) {
      return 'Нет ответа от сервера. Проверьте подключение к интернету.';
    }
    
    // ⚙️ Сценарий В: Ошибка при настройке запроса (невалидный URL, баг в коде)
    return `Ошибка конфигурации запроса: ${error.message}`;
  }

  // 🆘 Сценарий Г: Не-Axios ошибка (например, синтаксическая или брошенная вручную)
  return error instanceof Error ? error.message : 'Произошла неизвестная ошибка';
};