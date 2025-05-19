using MongoDB_Integration.structures;
using OutSystems.ExternalLibraries.SDK;


namespace MongoDB_ODC
{
    [OSInterface(Description = @"MongoDB Connector for OutSystems (Unofficial) 
Integrate MongoDB with OutSystems apps via streamlined CRUD operations, advanced queries, and config-driven workflows.

CRUD & Pagination: Create, read, update, delete documents; fetch data in batches.

Aggregation Framework: Run complex pipelines (grouping, filtering) and analyze performance.

Config-Driven: Securely manage connections (MongoConfig) for stateless, scalable operations.

Ideal for dynamic apps, data analysis, and large dataset management. 🚀", Name = "MongoDB_Conector", IconResourceName = "MongoDB_Integration.resources.mongodb.ico")]
    public interface IMongoDB
    {
        // stateless. Any state or context needed to execute the external library should be passed explicitly as an input parameter.
        // usamos o MongoConfig e outros arquivos de /structures para receber os parâmetros de configuração do banco de dados ele e uma structure com decoratos para outystems, contem dados como ConnectionString, databaseName, collectionName
        [OSAction(Description = "Creates a document in the specified MongoDB collection")]
        MongoDBConectorResponse CreateDocument(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,
            [OSParameter(Description = "JSON document to insert. ex: { name: 'John', age: 30 }")] string documentJson,
            [OSParameter(Description = "ID da sessão (opcional, para controle de transação)")]string? sessionId = null);
            

        [OSAction(Description = "Retrieves documents from a MongoDB collection based on a filter")]
        MongoDBConectorResponse GetDocuments(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,
            [OSParameter(Description = "JSON filter for documents. ex: { name: 'John' }")] string filterJson);

        [OSAction(Description = "Retrieves a paginated list of documents from a MongoDB collection. The skip parameter specifies the number of documents to skip, and the limit parameter specifies the maximum number of documents to return.")]
        MongoDBConectorResponse GetPagedDocuments(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,
            [OSParameter(Description = "Number of documents to skip")] int skip,
            [OSParameter(Description = "Limit of documents to return")] int limit);

        [OSAction(Description = "Updates documents in a MongoDB collection based on a filter")]
        MongoDBConectorResponse UpdateDocument(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,
            [OSParameter(Description = "JSON filter for documents to update. ex: { name: 'John' }")] string filterJson,
            [OSParameter(Description = "JSON update definition. ex: { $set: { age: 31 } }")] string updateJson,
            [OSParameter(Description = "ID da sessão (opcional, para controle de transação)")]string? sessionId = null);

        [OSAction(Description = "Deletes documents from a MongoDB collection based on a filter")]
        MongoDBConectorResponse DeleteDocument(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,
            [OSParameter(Description = "JSON filter for documents to delete")] string filterJson,
            [OSParameter(Description = "ID da sessão (opcional, para controle de transação)")]string? sessionId = null);

        [OSAction(Description = "Returns the explain plan for an aggregation operation in MongoDB")]
        MongoDBConectorResponse AggregateExplainer(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,
            [OSParameter(Description = "Aggregation pipeline as JSON")] string aggregatePipeline,
            [OSParameter(Description = "Verbose output flag")] bool verbose);

        [OSAction(Description = "Performs an aggregation query on a MongoDB collection")]
        MongoDBConectorResponse AggregateCollection(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,
            [OSParameter(Description = "Aggregation pipeline as JSON. Ex: [{ \"$match\": { \"status\": \"A\" } }, { \"$group\": { \"_id\": \"$category\", \"total\": { \"$sum\": \"$amount\" } } }]")] string aggregatePipeline,
            [OSParameter(Description = "ID da sessão (opcional, para controle de transação)")]string? sessionId = null);
            

        [OSAction(Description = "Retrieves statistics for a MongoDB collection")]
        MongoDBConectorResponse GetCollectionStats(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config);

        [OSAction(Description = "Retrieves index information for a MongoDB collection")]
        MongoDBConectorResponse GetIndexInfo(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config);

        [OSAction(Description = "Retrieves a document by its ID from a MongoDB collection")]
        MongoDBConectorResponse GetDocumentById(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,
            [OSParameter(Description = "ID of the document to retrieve")] string documentId);


        [OSAction(Description = "Retrieves the number of documents in a MongoDB collection")]
        MongoDBConectorResponse CountDocuments(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,
            [OSParameter(Description = "JSON filter for documents. ex: { name: 'John' }")] string filterJson,
            [OSParameter(Description = "explain")] bool explain);


        [OSAction(Description = "Checks if a document exists in a MongoDB collection")]
        MongoDBConectorResponse IsDocumentExist(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,
            [OSParameter(Description = "JSON filter for documents. ex: { name: 'John' }")] string filterJson);

        [OSAction(Description = "Comita uma transação aberta")]
        MongoDBConectorResponse MongoDBCommitTransaction(
            [OSParameter(Description = "Configuração MongoDB")] MongoConfig config,
            [OSParameter(Description = "ID da sessão de transação")] string sessionId);

        [OSAction(Description = "Aborta uma transação aberta")]
        MongoDBConectorResponse MongoDBAbortTransaction(
            [OSParameter(Description = "Configuração MongoDB")] MongoConfig config,
            [OSParameter(Description = "ID da sessão de transação")] string sessionId);


    }
}