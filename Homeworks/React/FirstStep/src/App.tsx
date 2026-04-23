import { useState } from "react";
import axios from "axios";
import "./App.css";
import { NotificationBox } from "./components/NotificationBox";
import type { CatFactResponse } from "./types/api";
import { formatAxiosError } from "./utils/apiErrors";

function App() {
  const [fact, setFact] = useState<string>("");
  const [error, setError] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);

  // Функция запроса
  const fetchCatFact = async () => {
    setLoading(true);
    setError("");
    setFact("");

    try {
      const response = await axios.get<CatFactResponse>(
        "https://catfact.ninja/fact"
        //"https://catfact.ninja/fact-invalid" //для теста ошибки
      );

      // Если мы здесь — статус точно 2xx
      setFact(response.data.fact);
    } catch (err) {
      const message = formatAxiosError(err);
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ padding: "24px", fontFamily: "Arial" }}>
      <h1>🐱 Cat Facts</h1>

      <button onClick={fetchCatFact} disabled={loading}>
        {loading ? "Загрузка..." : "Получить факт"}
      </button>

      {fact && <NotificationBox text={fact} variant="success" />}
      {error && <NotificationBox text={error} variant="error" />}
    </div>
  );
}

export default App;
