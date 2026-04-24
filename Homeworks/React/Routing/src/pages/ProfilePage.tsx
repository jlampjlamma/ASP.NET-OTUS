// src/pages/ProfilePage.tsx

import { withAuth } from '../hoc/withAuth';

// 📦 FETCH LEGACY / OLD LOGIC: Закомментированная дублирующаяся проверка
/*
import { Navigate, useLocation } from 'react-router-dom';
import { useAppSelector } from '../hooks/redux';

const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
const location = useLocation();
if (!isAuthenticated) {
  return <Navigate to="/login" state={{ from: location.pathname }} replace />;
}
*/

//  AXIOS PRODUCTION / HOC: Чистый компонент, отвечающий только за UI
// eslint-disable-next-line react-refresh/only-export-components
function ProfilePage() {
  return (
    <div style={{ padding: '2rem' }}>
      <h1>👤 Личный кабинет</h1>
      <p>Здесь отображается приватная информация пользователя.</p>
      <p>Токен, настройки профиля, история заказов...</p>
    </div>
  );
}

// 🛡 Применяем HOC: экспортируем уже "защищённую" версию компонента
export default withAuth(ProfilePage);