namespace Market.Config
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public Collections Collections { get; set; }
        public int ConnectionTimeout { get; set; }
        public int MaxConnectionPoolSize { get; set; }
    }

    public class Collections
    {
        public string Admins { get; set; }
        public string Products { get; set; }
        public string Orders { get; set; }
    }
} 