// ...existing code...
using System.Data.Common;

namespace WebApplication2.Data
{
    public interface IDbConnectionFactory
    {
        DbConnection CreateConnection();
    }
}