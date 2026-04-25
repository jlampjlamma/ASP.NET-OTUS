import { useState } from 'react'
import axios from 'axios'
import './App.css'

interface JokeWeather {
  date: string;
  temperature: number;
  scaleName: string;
  weatherType: string;
  forecastPhrase: string;
}

const API_URL = 'https://localhost:7112/api/jokeweather'

function App() {
  const [weather, setWeather] = useState<JokeWeather | null>(null)
  const [loading, setLoading] = useState<boolean>(false)
  const [error, setError] = useState<string | null>(null)

  const fetchWeather = async () => {
    setLoading(true)
    setError(null)
    try {
      const response = await axios.get<JokeWeather>(API_URL)
      setWeather(response.data)
    } catch (err) {
      setError('Не удалось получить прогноз:' + err)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="container">
      <h1>🎭 Шуточный прогноз</h1>

      {loading && <p className="status">⏳ Загружаем данные...</p>}
      {error && <p className="error">{error}</p>}

      {weather && !loading && (
        <div className="forecast">
          <div className="temp-row">
            <span className="temp">{weather.temperature > 0 ? '+' : ''}{weather.temperature}</span>
            <span className="scale">{weather.scaleName}</span>
          </div>
          <p className="phrase">{weather.forecastPhrase}</p>
        </div>
      )}

      <button onClick={fetchWeather} disabled={loading}>
        {loading ? 'Загрузка...' : 'Узнать погоду'}
      </button>
    </div>
  )
}

export default App