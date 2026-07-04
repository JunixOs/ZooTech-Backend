using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using ZooTech.Infrastructure.Exceptions;

namespace ZooTech.Infrastructure.Caching
{
    public class RedisCacheConnection
    {
        private readonly ConnectionMultiplexer _connection;
        public RedisCacheConnection(
            IConfiguration configuration
        )
        {
            var redisConnectionString = configuration["Redis:ConnectionString"] ?? throw new UndefinedConfigurationValue(
                    message: "Missing configuration: Redis:ConnectionString"
                );

            _connection = ConnectionMultiplexer.Connect(redisConnectionString);
        }

        public IDatabase GetDatabase()
        {
            return _connection.GetDatabase();
        }
    }
}