namespace MicroServices.Shared
{
    public class AppSettings
    {
        public string AppPath { get; set; }
        public string PlatformName { get; set; }
        public string FriendlyName { get; set; }
        public bool EnableSwagger { get; set; }
        public bool EnableHealthCheck { get; set; }
        public bool EnableHttps { get; set; }
        public bool EnableErrorHandlerMiddleware { get; set; }
        public bool EnableRequestLogging { get; set; }
        public bool EnableAutoMigration { get; set; }
        public bool InjectSqlScriptsOnStartup { get; set; }
        public string CacheConnection { get; set; }
    }

    public enum CacheType
    {
        Distributed = 1,
        Memory = 2
    }
}
