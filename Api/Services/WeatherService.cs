using Api.Models;
using Api.Repositories;

namespace Api.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _repo;

        public WeatherService(IWeatherRepository repo) => _repo = repo;

        public async Task<IEnumerable<WeatherForecast>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<WeatherForecast?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<WeatherForecast> CreateAsync(WeatherForecast forecast)
        {
            await _repo.AddAsync(forecast);
            await _repo.SaveChangesAsync();
            return forecast;
        }

        public async Task<bool> UpdateAsync(int id, WeatherForecast forecast)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Date = forecast.Date;
            existing.TemperatureC = forecast.TemperatureC;
            existing.TemperatureF = forecast.TemperatureF;
            existing.Summary = forecast.Summary;

            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            await Task.Run(() => _repo.DeleteAsync(existing));
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
