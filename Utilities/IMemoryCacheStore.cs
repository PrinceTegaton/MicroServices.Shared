using System;
using System.Collections.Generic;

namespace MicroServices.Shared.Utilities
{
    public interface IMemoryCacheStore
    {
        void Add(string key, object value, DateTime? expiresOn = null, bool isSlidingExpiration = false);
        void Add(string key, string value, DateTime? expiresOn = null, bool isSlidingExpiration = false);
        void Remove(string key);
        T GetObject<T>(string key);
        IEnumerable<T> GetList<T>(string key);
        decimal? GetDecimal(string key);
        double? GetDouble(string key);
        int GetInt(string key);
        long GetLong(string key);
        string GetString(string key);
    }
}