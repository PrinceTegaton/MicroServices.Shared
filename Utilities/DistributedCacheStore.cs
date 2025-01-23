using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MicroServices.Shared.Utilities
{
    public class DistributedCacheStore : IDistributedCacheStore
    {
        public readonly IDatabase _redisStore;

        public DistributedCacheStore(ConnectionMultiplexer redisConnection)
        {
            _redisStore = redisConnection.GetDatabase();
        }

        public bool KeyExist(string key, string parentKey = null)
        {
            if (string.IsNullOrEmpty(parentKey))
            {
                return _redisStore.KeyExists(key);
            }
            else
            {
                return _redisStore.HashExists(parentKey, key);
            }
        }

        #region ADD STRINGS
        public void Add(string key, string value, DateTime? expiresOn = null, bool isSlidingExpiration = false)
        {
            // add plain string

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                return;
            }

            _redisStore.StringSet(key, value);

            if (expiresOn.HasValue)
            {
                _redisStore.KeyExpire(key, expiresOn);
            }

            if (isSlidingExpiration)
            {
                // todo: add logic
            }
        }

        public void AddWithParent(string parentKey, string key, string value, DateTime? expiresOn = null, bool isSlidingExpiration = false)
        {
            // add string in an hashset

            if (string.IsNullOrEmpty(parentKey) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                return;
            }

            _redisStore.HashSet(parentKey, new HashEntry[] { new HashEntry(key, value) });

            if (expiresOn.HasValue)
            {
                _redisStore.KeyExpire(key, expiresOn);
            }

            if (isSlidingExpiration)
            {
                // todo: add logic
            }
        }
        #endregion


        #region ADD OBJECTS
        public void Add(string key, object value, DateTime? expiresOn = null, bool isSlidingExpiration = false)
        {
            // add object in an hashset

            if (string.IsNullOrEmpty(key) || value == null)
            {
                return;
            }

            byte[] objectToCache = JsonSerializer.SerializeToUtf8Bytes(value);
            _redisStore.HashSet(key, "data", objectToCache);

            if (expiresOn.HasValue)
            {
                _redisStore.KeyExpire(key, expiresOn);
            }

            if (isSlidingExpiration)
            {
                // todo: add logic
            }
        }

        public void AddWithParent(string parentKey, string key, object value, DateTime? expiresOn = null, bool isSlidingExpiration = false)
        {
            if (string.IsNullOrEmpty(parentKey) || string.IsNullOrEmpty(key) || value == null)
            {
                return;
            }

            byte[] objectToCache = JsonSerializer.SerializeToUtf8Bytes(value);
            _redisStore.HashSet(parentKey, new HashEntry[] { new HashEntry(key, objectToCache) });

            if (expiresOn.HasValue)
            {
                _redisStore.KeyExpire(key, expiresOn);
            }

            if (isSlidingExpiration)
            {
                // todo: add logic
            }
        }

        public void AddWithParent(string parentKey, List<KeyValuePair<string, string>> dataset, DateTime? expiresOn = null, bool isSlidingExpiration = false)
        {
            if (string.IsNullOrEmpty(parentKey) || dataset == null || !dataset.Any())
            {
                return;
            }

            _redisStore.HashSet(parentKey, dataset.Select(a => new HashEntry(a.Key, a.Value)).ToArray());


            if (expiresOn.HasValue)
            {
                _redisStore.KeyExpire(parentKey, expiresOn);
            }

            if (isSlidingExpiration)
            {
                // todo: add logic
            }
        }

        public void AddWithParent(string parentKey, List<KeyValuePair<string, object>> dataset, DateTime? expiresOn = null, bool isSlidingExpiration = false)
        {
            if (string.IsNullOrEmpty(parentKey) || dataset == null || !dataset.Any())
            {
                return;
            }

            _redisStore.HashSet(parentKey, dataset.Select(a => new HashEntry(a.Key, JsonSerializer.SerializeToUtf8Bytes(a.Value))).ToArray());


            if (expiresOn.HasValue)
            {
                _redisStore.KeyExpire(parentKey, expiresOn);
            }

            if (isSlidingExpiration)
            {
                // todo: add logic
            }
        }
        #endregion


        public void Remove(string key, string parentKey = null)
        {
            if (string.IsNullOrEmpty(parentKey))
            {
                _redisStore.KeyDelete(key);
            }
            else
            {
                _redisStore.HashDelete(parentKey, key);
            }
        }

        private T getObject<T>(string key, string parentKey = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default;
            }

            string value = string.Empty;

            if (string.IsNullOrEmpty(parentKey))
            {
                value = _redisStore.HashGet(key, "data");
            }
            else
            {
                value = _redisStore.HashGet(parentKey, key);
            }


            if (value == null || value.Length == 0)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value);
        }

        private string getString(string key, string parentKey = null)
        {
            if (string.IsNullOrEmpty(parentKey))
            {
                return _redisStore.StringGet(key);
            }
            else
            {
                return _redisStore.HashGet(parentKey, key);
            }
        }

        public T GetObject<T>(string key, string parentKey = null)
        {
            return getObject<T>(key, parentKey);
        }

        public IEnumerable<T> GetList<T>(string key, string parentKey = null)
        {
            return getObject<IEnumerable<T>>(key, parentKey);
        }

        public string GetString(string key, string parentKey = null)
        {
            return getString(key, parentKey);
        }

        public decimal? GetDecimal(string key, string parentKey = null)
        {
            return Convert.ToDecimal(getString(key, parentKey));
        }

        public double? GetDouble(string key, string parentKey = null)
        {
            return Convert.ToDouble(getString(key, parentKey));
        }

        public int GetInt(string key, string parentKey = null)
        {
            return Convert.ToInt32(getString(key, parentKey));
        }

        public long GetLong(string key, string parentKey = null)
        {
            return Convert.ToInt64(getString(key, parentKey));
        }
    }
}