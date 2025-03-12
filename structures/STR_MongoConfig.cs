using OutSystems.ExternalLibraries.SDK;

namespace MongoDB_ODC 
{
    [OSStructure(Description = "Configuração avançada de conexão MongoDB")]
    public struct MongoConfig 
    {
        [OSStructureField(DataType = OSDataType.Text, Description = "Connection string do MongoDB", IsMandatory = true)]
        public string ConnectionString { get; set; }

        [OSStructureField(DataType = OSDataType.Text, Description = "Nome do banco de dados", IsMandatory = true)]
        public string DatabaseName { get; set; }

        [OSStructureField(DataType = OSDataType.Text, Description = "Nome da coleção", IsMandatory = true)]
        public string CollectionName { get; set; }

        [OSStructureField(DataType = OSDataType.Integer, Description = "Tamanho máximo do pool de conexões", IsMandatory = false)]
        public int? MaxPoolSize { get; set; }

        [OSStructureField(DataType = OSDataType.Integer, Description = "Timeout de conexão em segundos", IsMandatory = false)]
        public int? ConnectTimeout { get; set; }

        [OSStructureField(DataType = OSDataType.Boolean, Description = "Habilita SSL", IsMandatory = false)]
        public bool? UseSSL { get; set; }

        public MongoConfig(
            string connectionString,
            string databaseName,
            string collectionName,
            int? maxPoolSize = null,
            int? connectTimeout = 30,
            bool? useSSL = true)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            CollectionName = collectionName;
            MaxPoolSize = maxPoolSize >= 1 ? maxPoolSize : throw new ArgumentOutOfRangeException(nameof(maxPoolSize), "MaxPoolSize must be 1 or greater.");
            ConnectTimeout = connectTimeout;
            UseSSL = useSSL;
        }
    }
}