// src/pages/SettingsPage.tsx

import { withAuth } from '../hoc/withAuth';

// 📦 OLD LOGIC: Закомментированная копия проверки
/*
import { Navigate, useLocation } from 'react-router-dom';
import { useAppSelector } from '../hooks/redux';

const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
const location = useLocation();
if (!isAuthenticated) {
  return <Navigate to="/login" state={{ from: location.pathname }} replace />;
}
*/

// 🚀 HOC: Компонент занимается только отображением настроек
// eslint-disable-next-line react-refresh/only-export-components
function SettingsPage() {
  return (
    <div style={{ padding: '2rem' }}>
      <h1>⚙️ Настройки</h1>
      <p>Здесь можно сменить пароль, email или тему оформления.</p>
      <p>Доступно только авторизованным пользователям.</p>
    </div>
  );
}

// 🛡 Применяем HOC
export default withAuth(SettingsPage);