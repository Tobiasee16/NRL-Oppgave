// ...existing code...
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public interface IObstacleRepository
    {
        Task<int> InsertAsync(ObstacleData obstacle);
        Task<ObstacleData?> GetByIdAsync(int id);
        Task<IEnumerable<ObstacleData>> ListAsync();
    }
}