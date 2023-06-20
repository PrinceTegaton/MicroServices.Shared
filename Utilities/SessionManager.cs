using System;

namespace MicroServices.Shared.Utilities;

public class SessionManager
{
    private readonly IDistributedCacheStore _cache;

    public SessionManager(IDistributedCacheStore cache)
    {
        _cache = cache;
    }

    public string Get(string sessionName, string key)
    {
        return _cache.GetString(key, sessionName);
    }

    public string Get(string key)
    {
        return _cache.GetString(key);
    }

    public DateTime? GetEntryDate(string sessionName, string key)
    {
        return _cache.GetObject<SessionObject>(key, sessionName)?.dt;
    }

    public bool Exists(string sessionName, string key)
    {
        return _cache.KeyExist(key, sessionName);
    }

    public (bool status, string msg) Create(string sessionName, string key, string value, bool checkExisting = true, DateTime? expiresOn = null, bool isSliding = false)
    {
        if (checkExisting && Exists(sessionName, key))
        {
            return (false, "Duplicate session rejected");
        }

        _cache.AddWithParent(sessionName, key, value, expiresOn, isSliding);
        return (true, "Created");
    }

    public (bool status, string msg) Create(string sessionName, string key, object value, bool checkExisting = true, DateTime? expiresOn = null, bool isSliding = false)
    {
        if (checkExisting && Exists(sessionName, key))
        {
            return (false, "Duplicate session rejected");
        }

        _cache.AddWithParent(sessionName, key, value, expiresOn, isSliding);
        return (true, "Created");
    }

    public (bool status, string msg) Create(string key, string value, bool checkExisting = true, DateTime? expiresOn = null, bool isSliding = false)
    {
        if (checkExisting && _cache.KeyExist(key))
        {
            return (false, "Duplicate session rejected");
        }

        _cache.Add(key, value, expiresOn, isSliding);
        return (true, "Created");
    }

    public (bool status, string msg) Create(string key, object value, bool checkExisting = true, DateTime? expiresOn = null, bool isSliding = false)
    {
        if (checkExisting && _cache.KeyExist(key))
        {
            return (false, "Duplicate session rejected");
        }

        _cache.Add(key, value, expiresOn, isSliding);
        return (true, "Created");
    }

    public void Remove(string sessionName, string key)
    {
        _cache.Remove(key, sessionName);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}

public class SessionObject
{
    public DateTime? dt { get; set; }
}
