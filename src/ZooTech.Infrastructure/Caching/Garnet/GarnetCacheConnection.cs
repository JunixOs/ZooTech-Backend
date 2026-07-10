using StackExchange.Redis;

namespace ZooTech.Infrastructure.Caching
{
    public class GarnetCacheConnection
    {
        private readonly IConnectionMultiplexer _connection;

        public GarnetCacheConnection(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public IDatabase GetDatabase()
        {
            return _connection.GetDatabase();
        }

        public IConnectionMultiplexer GetMultiplexer()
        {
            return _connection;
        }
    }
}
