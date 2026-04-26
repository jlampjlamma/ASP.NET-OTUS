using RestApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JokeWeatherController : ControllerBase
    {
        private static readonly string[] TemperatureScales =
        [
            "горящего пердака",
            "сломанных тестов",
            "дедлайнов",
            "кофеина в крови",
            "закомментированного кода",
            "легаси-кода",
            "гит-конфликтов",
            "OutOfMemory",
            "нервов тимлида",
            "битого трафика",
            "продакшен-крашей",
            "неотловленных исключений"
    ];

        private static readonly Dictionary<string, string[]> WeatherPhrases = new()
    {
        {
            "Солнечно",
            new[]
            {
                "возможна успешная сдача спринта",
                "идеальное время для рефакторинга (но вы всё равно не будете)",
                "можно даже задеплоить без молитвы"
            }
        },
        {
            "Дождливо",
            new[]
            {
                "возможны осадки в виде 2-3 десятков багов",
                "возможны кратковременные слёзы джуна",
                "ожидается фидбек от тестировщиков",
            }
        },
        {
            "Снег",
            new[]
            {
                "ожидаются новые требования \"надо вчера\"",
                "ожидаются очередные правки от заказчика",
                "возможны локальные заморозки мотивации"
            }
        },
        {
            "Гроза",
            new[]
            {
                "возможно, сегодня ляжет прод",
                "ожидается истерика тимлида",
                "спасайся кто может — rollback не поможет",
                "возможно синьор сегодня напьется"
            }
        },
        {
            "Метель",
            new[]
            {
                "ожидаются внеочередные хотелки менагера",
                "видимость нулевая — как в твоем будущем",
                "заметает технический долг"
            }
        }
    };

        private static readonly Random Rnd = new();

        [HttpGet]
        public ActionResult<JokeWeatherResponse> GetForecast()
        {
            // 🔹 Генерируем температуру от -25 до +25 включительно
            var temperature = Rnd.Next(-25, 26); // верхняя граница не включается, поэтому 26

            // 🔹 Случайная шкала
            var scale = TemperatureScales[Rnd.Next(TemperatureScales.Length)];

            // 🔹 Случайный тип погоды
            var weatherKeys = WeatherPhrases.Keys.ToArray();
            var weatherType = weatherKeys[Rnd.Next(weatherKeys.Length)];

            // 🔹 Случайная фраза для выбранной погоды
            var phrases = WeatherPhrases[weatherType];
            var phrase = phrases[Rnd.Next(phrases.Length)];

            return new JokeWeatherResponse(
                Date: DateTime.Now,
                Temperature: temperature,
                ScaleName: scale,
                WatherType: weatherType,
                ForecastPhrase: phrase
            );
        }

        [HttpGet("batch/{count}")]
        public ActionResult<IEnumerable<JokeWeatherResponse>> GetBatchForecast(int count)
        {
            // Ограничиваем, чтобы не перегружать
            var limit = Math.Min(Math.Max(count, 1), 10);

            var result = new List<JokeWeatherResponse>();
            for (int i = 0; i < limit; i++)
            {
                result.Add(GetForecast().Value);
            }
            return result;
        }
    }
}
