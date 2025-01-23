
using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Models;

[ApiController]
[Route("weatherforcast")]
public class WeatherForcastController : ControllerBase
{

    private String[] summaries = new[]
        {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
  };

     public ActionResult<IEnumerable<WeatherForecast>>  GetWeatherForecast()
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }

}

internal interface IEnumerableatherForecast
{
}