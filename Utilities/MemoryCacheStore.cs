using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace MicroServices.Shared.Utilities
{
    public class MemoryCacheStore : IMemoryCacheStore
    {
        public readonly IMemoryCache _cache;

        public MemoryCacheStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Add(string key, object value, DateTime? expiresOn = null, bool isSlidingExpiration = false)
        {
            if (value == null)
            {
                return;
            }

            _cache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiresOn,
                SlidingExpiration = expiresOn.HasValue && isSlidingExpiration ?
                                        TimeSpan.FromTicks(expiresOn.Value.Ticks) : null
            });
        }

        public void Add(string key, string value, DateTime? expiresOn = null, bool isSlidingExpiration = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            _cache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiresOn,
                SlidingExpiration = expiresOn.HasValue && isSlidingExpiration ?
                                        TimeSpan.FromTicks(expiresOn.Value.Ticks) : null
            });
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        private T getObject<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }

            _cache.TryGetValue(key, out T item);
            return item;
        }

        private string getString(string key)
        {
            _cache.TryGetValue(key, out string v);
            return v;
        }

        public T GetObject<T>(string key)
        {
            return getObject<T>(key);
        }

        public IEnumerable<T> GetList<T>(string key)
        {
            return getObject<IEnumerable<T>>(key);
        }

        public string GetString(string key)
        {
            return getString(key);
        }

        public decimal? GetDecimal(string key)
        {
            return Convert.ToDecimal(getString(key));
        }

        public double? GetDouble(string key)
        {
            return Convert.ToDouble(getString(key));
        }

        public int GetInt(string key)
        {
            return Convert.ToInt32(getString(key));
        }

        public long GetLong(string key)
        {
            return Convert.ToInt64(getString(key));
        }
    }
}