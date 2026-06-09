using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace ZooTech.Infrastructure.Caching
{
    public class GarnetCacheConnection
    {
        private readonly ConnectionMultiplexer _connection;

        public GarnetCacheConnection(IConfiguration configuration)
        {
            var garnetConnectionString = configuration["Garnet:ConnectionString"] ?? throw new InvalidOperationException(
                    "Garnet:ConnectionString no configurado");

            _connection = ConnectionMultiplexer.Connect(garnetConnectionString);
        }

        public IDatabase GetDatabase()
        {
            return _connection.GetDatabase();
        }
    }
}
