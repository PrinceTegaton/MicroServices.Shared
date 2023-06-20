using System;
using System.Collections.Generic;

namespace MicroServices.Shared.Utilities
{
    public interface IDistributedCacheStore
    {
        /// <summary>
        /// Store object
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresOn"></param>
        /// <param name="isSlidingExpiration"></param>
        void Add(string key, object value, DateTime? expiresOn = null, bool isSlidingExpiration = false);

        /// <summary>
        /// Store string
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresOn"></param>
        /// <param name="isSlidingExpiration"></param>
        void Add(string key, string value, DateTime? expiresOn = null, bool isSlidingExpiration = false);

        /// <summary>
        /// Store hashset of plain strings
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="dataset"></param>
        /// <param name="expiresOn"></param>
        /// <param name="isSlidingExpiration"></param>
        void AddWithParent(string parentKey, List<KeyValuePair<string, string>> dataset, DateTime? expiresOn = null, bool isSlidingExpiration = false);

        /// <summary>
        /// Store hashset of objects
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="dataset"></param>
        /// <param name="expiresOn"></param>
        /// <param name="isSlidingExpiration"></param>
        void AddWithParent(string parentKey, List<KeyValuePair<string, object>> dataset, DateTime? expiresOn = null, bool isSlidingExpiration = false);

        /// <summary>
        /// Store hashset of object
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresOn"></param>
        /// <param name="isSlidingExpiration"></param>
        void AddWithParent(string parentKey, string key, object value, DateTime? expiresOn = null, bool isSlidingExpiration = false);

        /// <summary>
        /// Store hashset of plain string
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresOn"></param>
        /// <param name="isSlidingExpiration"></param>
        void AddWithParent(string parentKey, string key, string value, DateTime? expiresOn = null, bool isSlidingExpiration = false);

        decimal? GetDecimal(string key, string parentKey = null);
        double? GetDouble(string key, string parentKey = null);
        int GetInt(string key, string parentKey = null);
        IEnumerable<T> GetList<T>(string key, string parentKey = null);
        long GetLong(string key, string parentKey = null);
        T GetObject<T>(string key, string parentKey = null);
        string GetString(string key, string parentKey = null);
        bool KeyExist(string key, string parentKey = null);
        void Remove(string key, string parentKey = null);
    }
}