using Dapper;
using MySqlConnector;
using System.Data;
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
  (ObstacleName, ObstacleHeight, ObstacleDescription, Latitude, Longitude, GeometryGeoJson, CreatedAt, Status, ReporterEmail, SubmittedByUserId)
VALUES
  (@ObstacleName, @ObstacleHeight, @ObstacleDescription, @Latitude, @Longitude, @GeometryGeoJson, @CreatedAt, @Status, @ReporterEmail, @SubmittedByUserId);
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

        public async Task UpdateStatusAsync(int id, string status)
        {
            const string sql = "UPDATE `obstacles` SET Status = @Status WHERE Id = @Id;";
            await using var conn = _factory.CreateConnection();
            if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
            await conn.ExecuteAsync(sql, new { Id = id, Status = status });
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM `obstacles` WHERE Id = @Id;";
            await using var conn = _factory.CreateConnection();
            if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
            await conn.ExecuteAsync(sql, new { Id = id });
        }

        public async Task UpdateAsync(ObstacleData obstacle)
        {
            const string sql = @"
UPDATE `obstacles`
SET ObstacleName        = @ObstacleName,
    ObstacleHeight      = @ObstacleHeight,
    ObstacleDescription = @ObstacleDescription,
    GeometryGeoJson     = @GeometryGeoJson,
    Latitude            = @Latitude,
    Longitude           = @Longitude
WHERE Id = @Id;
";

            using var conn = _factory.CreateConnection();
            if (conn.State == ConnectionState.Closed)
                await conn.OpenAsync();

            await conn.ExecuteAsync(sql, obstacle);
        }

        public async Task<IEnumerable<ObstacleData>> GetByReporterAsync(string userId)
        {
            const string sql = @"
SELECT *
FROM `obstacles`
WHERE SubmittedByUserId = @UserId
ORDER BY CreatedAt DESC;";

            using var conn = _factory.CreateConnection();
            if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
            return await conn.QueryAsync<ObstacleData>(sql, new { UserId = userId });
        }
    }
}
