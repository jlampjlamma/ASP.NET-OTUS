
import "./App.css";
import { NotificationBox } from "./components/NotificationBox";
import { useCatFact } from "./hooks/useCatFact";

function App() {
  const { fact, error, loading, fetchFact } = useCatFact();

  return (
    <div style={{ padding: "24px", fontFamily: "Arial" }}>
      <h1>🐱 Cat Facts</h1>

      <button onClick={fetchFact} disabled={loading}>
        {loading ? "Загрузка..." : "Получить факт"}
      </button>

      {fact && <NotificationBox text={fact} variant="success" />}
      {error && <NotificationBox text={error} variant="error" />}
    </div>
  );
}

export default App;
