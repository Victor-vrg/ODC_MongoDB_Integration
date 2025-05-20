using OutSystems.ExternalLibraries.SDK;

namespace MongoDB_Integration.structures
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

        [OSStructureField(DataType = OSDataType.Boolean, Description = "Habilita SSL", IsMandatory = false, DefaultValue = "True")]
        public bool? UseSSL { get; set; }

        [OSStructureField(DataType = OSDataType.Integer, Description = "Timeout padrão para transações em segundos", IsMandatory = false, DefaultValue = "30")]
        public int TransactionDefaultTimeout { get; set; } = 30;

        [OSStructureField(DataType = OSDataType.Boolean, Description = "Habilita commit automático de transações", IsMandatory = false, DefaultValue = "True")]
        public bool AutoCommitTransactions { get; set; } = true;


        public MongoConfig(
            string connectionString,
            string databaseName,
            string collectionName,
            int? maxPoolSize = 100,
            int? connectTimeout = 60,
            bool? useSSL = true)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            CollectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
            MaxPoolSize = maxPoolSize.HasValue && maxPoolSize.Value >= 1 ? maxPoolSize : 60;
            ConnectTimeout = connectTimeout;
            UseSSL = useSSL;
        }
    }
}
