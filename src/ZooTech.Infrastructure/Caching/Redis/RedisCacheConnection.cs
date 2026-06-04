using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace ZooTech.Infrastructure.Caching
{
    public class RedisCacheConnection
    {
        private readonly ConnectionMultiplexer _connection;
        public RedisCacheConnection(
            IConfiguration configuration
        )
        {
            var redisConnectionString = configuration["Redis:ConnectionString"] ?? throw new InvalidOperationException(
                    "Redis:ConnectionString no configurado");

            _connection = ConnectionMultiplexer.Connect(redisConnectionString);
        }

        public IDatabase GetDatabase()
        {
            return _connection.GetDatabase();
        }
    }
}