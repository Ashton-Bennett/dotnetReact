using Api.Models;

namespace Api.Services
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecast>> GetAllAsync();
        Task<WeatherForecast?> GetByIdAsync(int id);
        Task<WeatherForecast> CreateAsync(WeatherForecast forecast);
        Task<bool> UpdateAsync(int id, WeatherForecast forecast);
        Task<bool> DeleteAsync(int id);
    }
}