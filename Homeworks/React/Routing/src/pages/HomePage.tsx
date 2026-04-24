// src/pages/HomePage.tsx

import { useEffect, useState } from 'react'; // Хуки для жизни компонента

// 🔄 ИМПОРТЫ: Обновляем для Axios-версии
// Импортируем сервис и типы (интерфейсы не меняются, они универсальны)
import { postsService, type Post } from '../api/posts.service';

// 🆕 Импортируем нашу утилиту для форматирования ошибок
// Вместо кастомного ApiError теперь используем универсальный форматтер
import { formatAxiosError } from '../utils/apiErrors';

// 📦 FETCH LEGACY: Старый импорт больше не нужен
// import { ApiError } from '../api/client';

export default function HomePage() {
  // ==========================================
  // 1. СОСТОЯНИЕ (STATE)
  // ==========================================
  
  // Храним список постов. Изначально пустой массив.
  const [posts, setPosts] = useState<Post[]>([]);
  
  // Флаг загрузки. Показываем спиннер, пока true.
  const [loading, setLoading] = useState(false);
  
  // Текст ошибки. Если null — всё хорошо, если строка — показываем пользователю.
  const [error, setError] = useState<string | null>(null);
  
  // Храним только что созданный пост, чтобы показать подтверждение
  const [createdPost, setCreatedPost] = useState<Post | null>(null);

  // ==========================================
  // 2. ЗАГРУЗКА ДАННЫХ (GET с параметрами)
  // ==========================================
  
  // useEffect с пустым массивом зависимостей [] срабатывает 1 раз при монтировании компонента
  useEffect(() => {
    // Объявляем асинхронную функцию ВНУТРИ эффекта (нельзя делать сам useEffect async)
    const loadPosts = async () => {
      setLoading(true);   // Включаем индикатор загрузки
      setError(null);     // Сбрасываем старую ошибку
      
      try {
        // ==========================================
        // 🚀 AXIOS PRODUCTION: Вызов сервиса
        // ==========================================
        // 🔑 Важное отличие от fetch:
        // Axios возвращает объект { data, status, headers, config, ... }
        // Нам нужны только данные, поэтому сразу деструктурируем .data
        const { data } = await postsService.getPosts({ userId: 1, _limit: 3 });
        
        // Обновляем стейт: сохраняем данные в posts
        // Это вызовет перерисовку компонента с новым списком
        setPosts(data);
        
      } catch (err) {
        // Если в сервисе или axios произошло исключение (throw), мы попадаем сюда
        
        // ==========================================
        // 📦 FETCH LEGACY: Старая обработка ошибок (закомментирована)
        // ==========================================
        /*
        // Проверяем, наша ли это ошибка (экземпляр класса ApiError)
        if (err instanceof ApiError) {
          // Берем понятное сообщение из ошибки
          setError(err.message);
        } else {
          // Если ошибка неизвестная (например, баг в коде), пишем общее сообщение
          setError('Произошла неизвестная ошибка');
        }
        */

        // ==========================================
        // 🚀 AXIOS PRODUCTION: Новая обработка ошибок
        // ==========================================
        /**
         * Почему используем formatAxiosError?
         * 
         * 1. Axios выбрасывает ошибку при ЛЮБОМ статусе не 2xx (404, 500 и т.д.)
         *    (fetch этого не делал, там нужна была ручная проверка !response.ok)
         * 
         * 2. Ошибка Axios имеет сложную структуру:
         *    - error.response → сервер ответил с ошибкой (есть статус и данные)
         *    - error.request → запрос ушёл, но ответа нет (сеть упала, таймаут)
         *    - error.message → ошибка настройки (неверный URL, баг в коде)
         * 
         * 3. Функция formatAxiosError разбирает эти случаи и возвращает
         *    человекочитаемое сообщение, готовое для показа пользователю.
         */
        
        // Вызываем утилиту: она вернёт строку с описанием проблемы
        const errorMessage = formatAxiosError(err);
        setError(errorMessage);
        
      } finally {
        // finally выполняется ВСЕГДА, независимо от успеха или провала
        // Выключаем индикатор загрузки
        setLoading(false);
      }
    };

    // Запускаем функцию загрузки
    loadPosts();
  }, []); // Пустой массив [] означает: "выполнить только при первом рендере"

  // ==========================================
  // 3. СОЗДАНИЕ ПОСТА (POST с контрактом)
  // ==========================================
  
  // Функция-обработчик для кнопки "Создать"
  const handleCreatePost = async () => {
    try {
      // Подготавливаем данные строго по контракту CreatePostPayload
      const payload = {
        title: 'Мой первый пост через API',
        body: 'Это тело поста, отправленное с фронтенда',
        userId: 1,
      };

      // ==========================================
      // 🚀 AXIOS PRODUCTION: Вызов сервиса с телом
      // ==========================================
      // 🔑 Важное отличие от fetch:
      // 1. Не нужно делать JSON.stringify() — Axios сам сериализует объект
      // 2. Не нужно ставить заголовок Content-Type — он уже в axios.ts
      // 3. Возвращается { data, ... }, поэтому деструктурируем .data
      const { data } = await postsService.createPost(payload);
      
      // Показываем пользователю, что пост создан
      setCreatedPost(data);
      
      // (Опционально) Добавляем новый пост в начало списка, чтобы он сразу появился
      setPosts(prev => [data, ...prev]);
      
    } catch (err) {
      // Обработка ошибки при создании (аналогично загрузке)
      
      // ==========================================
      // 📦 FETCH LEGACY: Старая обработка (закомментирована)
      // ==========================================
      /*
      if (err instanceof ApiError) {
        setError(err.message);
      } else {
        setError('Не удалось создать пост');
      }
      */

      // ==========================================
      // 🚀 AXIOS PRODUCTION: Новая обработка
      // ==========================================
      // Используем ту же утилиту — она универсальна для любых запросов
      const errorMessage = formatAxiosError(err);
      setError(errorMessage);
    }
  };

  // ==========================================
  // 4. ОТРИСОВКА (JSX)
  // ==========================================
  return (
    <div style={{ padding: '2rem', maxWidth: '800px', margin: '0 auto' }}>
      <h1>Главная страница</h1>
      
      {/* Блок отображения ошибок: рендерится только если error не null */}
      {error && (
        <div style={{ color: '#b91c1c', padding: '0.75rem', background: '#fee2e2', borderRadius: '6px', marginBottom: '1rem' }}>
          ❌ {error}
        </div>
      )}

      {/* Секция 1: Список постов (результат GET) */}
      <section style={{ marginBottom: '2rem', padding: '1rem', border: '1px solid var(--border)', borderRadius: '8px' }}>
        <h2>📥 Посты пользователя (Загрузка с параметрами)</h2>
        
        {/* Условный рендеринг: если загрузка — показываем текст, иначе — список */}
        {loading ? (
          <p>⏳ Загрузка данных...</p>
        ) : (
          <ul style={{ listStyle: 'none', padding: 0, display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
            {/* Перебираем массив постов. key обязателен для списков в React */}
            {posts.map(post => (
              <li key={post.id} style={{ padding: '0.5rem', background: 'var(--code-bg)', borderRadius: '4px' }}>
                <strong>{post.title}</strong>
                <p style={{ margin: '0.25rem 0', fontSize: '0.9rem' }}>{post.body}</p>
                <small style={{ color: 'var(--text)' }}>Автор ID: {post.userId}</small>
              </li>
            ))}
            {/* Если список пуст (например, ошибка или нет данных) */}
            {posts.length === 0 && !loading && <p>Постов не найдено</p>}
          </ul>
        )}
      </section>

      {/* Секция 2: Кнопка создания (результат POST) */}
      <section style={{ padding: '1rem', border: '1px solid var(--border)', borderRadius: '8px' }}>
        <h2>📤 Создать пост (Отправка контракта)</h2>
        
        <button 
          onClick={handleCreatePost}
          style={{ padding: '0.5rem 1rem', cursor: 'pointer', background: 'var(--accent)', color: '#fff', border: 'none', borderRadius: '4px' }}
        >
          ✨ Создать тестовый пост
        </button>
        
        {/* Показываем результат создания, если newPost есть в стейте */}
        {createdPost && (
          <div style={{ marginTop: '1rem', padding: '0.75rem', background: 'var(--accent-bg)', borderRadius: '6px' }}>
            <p><strong>✅ Успешно создано:</strong></p>
            <p><strong>ID:</strong> {createdPost.id} (сгенерирован сервером)</p>
            <p><strong>Заголовок:</strong> {createdPost.title}</p>
          </div>
        )}
      </section>
    </div>
  );
}