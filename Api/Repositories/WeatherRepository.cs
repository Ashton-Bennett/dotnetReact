using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly AppDbContext _context;
        public WeatherRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<WeatherForecast>> GetAllAsync() =>
            await _context.WeatherForecasts.ToListAsync();

        public async Task<WeatherForecast?> GetByIdAsync(int id) =>
            await _context.WeatherForecasts.FindAsync(id);

        public async Task AddAsync(WeatherForecast forecast) =>
            await _context.WeatherForecasts.AddAsync(forecast);

        public Task UpdateAsync(WeatherForecast forecast)
        {
            _context.WeatherForecasts.Update(forecast);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(WeatherForecast forecast)
        {
            _context.WeatherForecasts.Remove(forecast);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
