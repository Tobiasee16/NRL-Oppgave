// ...existing code...
using Dapper;
using System.Data;
using System.Data.Common;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class ObstacleRepository : IObstacleRepository
    {
        private readonly IDbConnectionFactory _factory;

        public ObstacleRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<int> InsertAsync(ObstacleData obstacle)
        {
            const string insertSql = @"
INSERT INTO `obstacles`
  (ObstacleName, ObstacleHeight, ObstacleDescription, Latitude, Longitude, GeometryGeoJson, CreatedAt)
VALUES
  (@ObstacleName, @ObstacleHeight, @ObstacleDescription, @Latitude, @Longitude, @GeometryGeoJson, @CreatedAt);
SELECT LAST_INSERT_ID();";

            using var conn = _factory.CreateConnection();
            if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
            if (obstacle.CreatedAt == default) obstacle.CreatedAt = DateTime.UtcNow;
            var newId = await conn.ExecuteScalarAsync<int>(insertSql, obstacle);
            return newId;
        }

        public async Task<ObstacleData?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM `obstacles` WHERE Id = @id;";
            using var conn = _factory.CreateConnection();
            if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
            return await conn.QuerySingleOrDefaultAsync<ObstacleData>(sql, new { id });
        }

        public async Task<IEnumerable<ObstacleData>> ListAsync()
        {
            const string sql = "SELECT * FROM `obstacles` ORDER BY CreatedAt DESC;";
            using var conn = _factory.CreateConnection();
            if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
            return await conn.QueryAsync<ObstacleData>(sql);
        }
    }
}