namespace ASP.Server.DTO
{
    public record class JokeWeatherResponse(
        DateTimeOffset Date, 
        int Temperature, 
        string ScaleName, 
        string WatherType, 
        string ForecastPhrase);
}
