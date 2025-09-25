using Api.Models;

namespace Api.Repositories
{
    public interface IWeatherRepository
    {
        Task<IEnumerable<WeatherForecast>> GetAllAsync();
        Task<WeatherForecast?> GetByIdAsync(int id);
        Task AddAsync(WeatherForecast forecast);
        Task UpdateAsync(WeatherForecast forecast);
        Task DeleteAsync(WeatherForecast forecast);
        Task SaveChangesAsync();
    }
}
