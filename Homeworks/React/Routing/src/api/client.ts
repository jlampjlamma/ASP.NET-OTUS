// src/api/client.ts

/**
 * 📦 ФАЙЛ СОХРАНЕН КАК ЭТАЛОН БАЗОВОЙ РЕАЛИЗАЦИИ НА FETCH
 * 
 * Этот код демонстрирует, как работает сеть "под капотом":
 * - Ручная проверка response.ok
 * - Ручной JSON.stringify / JSON.parse
 * - Кастомный класс ApiError
 * 
 * В production-проектах этот функционал заменяется на axios.ts,
 * но понимание этого файла критически важно для отладки сетевых проблем.
 */

/**
 * Кастомный класс ошибки для единой обработки проблем с сетью.
 * Наследуемся от стандартного Error, чтобы можно было делать throw/catch.
 * 
 * ВАЖНО: Здесь мы используем "классический" синтаксис объявления свойств,
 * который совместим со всеми настройками TypeScript (включая erasableSyntaxOnly).
 */
export class ApiError extends Error {
  // 1. Объявляем свойства КЛАССА отдельно (а не в конструкторе)
  public readonly status: number;
  public readonly message: string;
  public readonly data?: unknown;

  /**
   * Конструктор принимает те же параметры, но теперь мы вручную присваиваем их свойствам.
   * Это чуть больше кода, но работает везде и понятнее для новичков.
   */
  constructor(status: number, message: string, data?: unknown) {
    // 2. Вызываем конструктор родительского класса (Error)
    // Передаем сообщение, чтобы оно было доступно через this.message и в консоли
    super(message);
    
    // 3. Важно для наследования от встроенных классов в современных версиях TS/JS
    this.name = 'ApiError';
    
    // 4. Вручную присваиваем параметры свойствам экземпляра класса
    // Теперь this.status, this.message, this.data доступны во всём классе
    this.status = status;
    this.message = message;
    this.data = data;
  }
}

/**
 * Базовый URL API. 
 * Можно вынести в .env файл для разных окружений.
 */
const API_BASE_URL = 'https://jsonplaceholder.typicode.com';

/**
 * Наш собственный тип опций, который НЕ наследуется от RequestInit.
 * Это даёт нам полный контроль над типами.
 */
type RequestOptions = {
  method?: RequestInit['method'];
  headers?: Record<string, string>;
  body?: unknown; // Любое значение: объект, массив, примитив
  signal?: AbortSignal; // Для отмены запроса
  // Можно добавить другие поля по необходимости
};

/**
 * Универсальная функция для выполнения любых HTTP-запросов.
 * 
 * @template T - Тип данных, который мы ожидаем получить в ответ.
 * @param url - Адрес эндпоинта.
 * @param options - Наши опции (не RequestInit!).
 * @returns Promise<T> - Промис с данными типа T.
 */
export async function request<T>(
  url: string, 
  options: RequestOptions = {}
): Promise<T> {
    // ✅ Добавляем базовый URL, если url не начинается с http
    const fullUrl = url.startsWith('http') 
    ? url 
    : `${API_BASE_URL}${url.startsWith('/') ? url : '/' + url}`;
  // 1. Сериализуем тело ЗАРАНЕЕ, до создания конфига для fetch
  // Теперь serializedBody имеет тип "string | undefined", что валидно для fetch
  const serializedBody = options.body !== undefined 
    ? JSON.stringify(options.body) 
    : undefined;

  // 2. Формируем конфиг для fetch, используя уже сериализованное тело
  const config: RequestInit = {
    method: options.method || 'GET',
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
    body: serializedBody, // ✅ string | undefined — это валидный BodyInit
    signal: options.signal,
  };

  try {
    const response = await fetch(fullUrl, config);

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new ApiError(
        response.status,
        (errorData as { message?: string }).message || `HTTP Error ${response.status}`,
        errorData
      );
    }

    if (response.status === 204 || response.headers.get('content-length') === '0') {
      return undefined as T;
    }

    return await response.json() as Promise<T>;

  } catch (error) {
    if (error instanceof TypeError) {
      throw new ApiError(0, 'Network error: unable to reach the server');
    }
    throw error;
  }
}