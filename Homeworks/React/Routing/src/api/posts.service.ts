// src/api/posts.service.ts

// 🔄 ИМПОРТ: Заменяем request из client.ts на axios инстанс
import apiClient from './axios';

// ==========================================
// 1. КОНТРАКТЫ (ТИПЫ) — БЕЗ ИЗМЕНЕНИЙ
// ==========================================

export interface Post {
  id: number;
  userId: number;
  title: string;
  body: string;
}

export interface CreatePostPayload {
  userId: number;
  title: string;
  body: string;
}

export interface GetPostsParams {
  userId?: number;
  _limit?: number;
}

// const API_BASE_URL = 'https://jsonplaceholder.typicode.com'; // 📦 FETCH LEGACY: константа больше не нужна

export const postsService = {
  
  // ==========================================
  // 📦 FETCH LEGACY: Старая реализация (закомментирована для истории)
  // ==========================================
  /*
  getPosts: (params?: GetPostsParams): Promise<Post[]> => {
    if (params) {
      const queryString = '?' + Object.entries(params)
        .filter(([, value]) => value !== undefined)
        .map(([key, value]) => `${key}=${value}`)
        .join('&');
      return request<Post[]>(`${API_BASE_URL}/posts${queryString}`);
    }
    return request<Post[]>(`${API_BASE_URL}/posts`);
  },

  createPost: (payload: CreatePostPayload): Promise<Post> => {
    return request<Post>(`${API_BASE_URL}/posts`, {
      method: 'POST',
      body: payload,
    });
  },
  */

  // ==========================================
  // 🚀 AXIOS PRODUCTION: Новая реализация
  // ==========================================
  
  /**
   * GET: Получить список постов
   * 
   * 🔑 Отличие от fetch: Axios принимает объект { params } и САМ собирает ?userId=1&_limit=3.
   * Не нужно вручную делать Object.entries().filter().map().join().
   */
  getPosts: (params?: GetPostsParams) => 
    apiClient.get<Post[]>('/posts', { params }),

  /**
   * POST: Создать новый пост
   * 
   * 🔑 Отличие от fetch: Axios САМ делает JSON.stringify(payload).
   * Мы просто передаём объект. Заголовок Content-Type уже настроен в axios.ts.
   */
  createPost: (payload: CreatePostPayload) => 
    apiClient.post<Post>('/posts', payload),
};