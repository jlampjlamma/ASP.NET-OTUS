// src/hoc/withAuth.tsx

import type { ComponentType } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAppSelector } from '../hooks/redux';

/**
 * Higher-Order Component (HOC) для защиты маршрутов.
 * 
 * Принимает компонент (WrappedComponent) и возвращает новый компонент (AuthGuard),
 * который проверяет авторизацию перед отрисовкой.
 * 
 * @template P - Тип пропсов оборачиваемого компонента
 * @param WrappedComponent - Компонент, который нужно защитить
 * @returns Новый компонент с встроенной проверкой авторизации
 */
export function withAuth<P extends object>(
  WrappedComponent: ComponentType<P>
): ComponentType<P> {
  
  // Возвращаем новый функциональный компонент
  const AuthGuard = (props: P) => {
    // 🔍 Читаем состояние авторизации из Redux
    const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
    // 📍 Запоминаем текущий путь, чтобы вернуть пользователя сюда после входа
    const location = useLocation();

    // 🚫 Если пользователь НЕ авторизован — редиректим на страницу входа
    // state={{ from: location.pathname }} позволяет после логина вернуться обратно
    // replace заменяет запись в истории, чтобы нельзя было нажать "Назад" на защищённую страницу
    if (!isAuthenticated) {
      return <Navigate to="/login" state={{ from: location.pathname }} replace />;
    }

    // ✅ Если авторизован — рендерим оригинальный компонент, пробрасывая все его пропсы
    return <WrappedComponent {...props} />;
  };

  // 🛠 Устанавливаем имя для React DevTools (помогает в отладке)
  const displayName = WrappedComponent.displayName || WrappedComponent.name || 'Component';
  AuthGuard.displayName = `withAuth(${displayName})`;

  return AuthGuard;
}