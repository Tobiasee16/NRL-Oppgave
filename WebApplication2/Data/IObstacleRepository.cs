// ...existing code...
using WebApplication2.Models;

// Definerer hvilke databaseoperasjoner som finnes for hinder
namespace WebApplication2.Data
{
    public interface IObstacleRepository
    {
        Task<int> InsertAsync(ObstacleData obstacle);
        Task<ObstacleData?> GetByIdAsync(int id);
        Task<IEnumerable<ObstacleData>> ListAsync();
        Task UpdateStatusAsync(int id, string status);
        Task DeleteAsync(int id);
        Task UpdateAsync(ObstacleData obstacle);
    }
}