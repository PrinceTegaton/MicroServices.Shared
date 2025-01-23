using StackExchange.Redis;
using System;

namespace MicroServices.Shared.Utilities
{
    public class RedisConnectionManager
    {
        public static ConnectionMultiplexer RedisConnection;

        public static ConnectionMultiplexer GetRedisConnection(string connectionString)
        {
            if (RedisConnection != null)
            {
                return RedisConnection;
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(connectionString, "Redis database connection string is required");
            }

            return RedisConnection = ConnectionMultiplexer.Connect(connectionString);
        }
    }
}
