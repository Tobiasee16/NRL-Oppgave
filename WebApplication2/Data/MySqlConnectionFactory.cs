// ...existing code...
using System.Data.Common;
using MySqlConnector;
using Microsoft.Extensions.Configuration;

namespace WebApplication2.Data
{
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public MySqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection");
        }

        public DbConnection CreateConnection() => new MySqlConnection(_connectionString);
    }
}