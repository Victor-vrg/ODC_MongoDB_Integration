using OutSystems.ExternalLibraries.SDK;


namespace MongoDB_ODC
{
    [OSInterface(Description = "Interface for MongoDB operations integrated with OutSystems", Name = "MongoDB_Conector", IconResourceName = "MongoDB_Integration.resources.mongodb.ico")]
    public interface IMongoDB
    {
        // usamos o MongoConfig e outros arquivos de /structures para receber os parâmetros de configuração do banco de dados ele e uma structure com decoratos para outystems, contem dados como ConnectionString, databaseName, collectionName
        [OSAction(Description = "Creates a document in the specified MongoDB collection")]
        ApiResponse CreateDocument(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,  
            [OSParameter(Description = "JSON document to insert. ex: { name: 'John', age: 30 }")] string documentJson);

        [OSAction(Description = "Retrieves documents from a MongoDB collection based on a filter")]
        ApiResponse GetDocuments(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,  
            [OSParameter(Description = "JSON filter for documents. ex: { name: 'John' }")] string filterJson);

        [OSAction(Description = "Retrieves a paginated list of documents from a MongoDB collection. The skip parameter specifies the number of documents to skip, and the limit parameter specifies the maximum number of documents to return.")]
        ApiResponse GetPagedDocuments(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,  
            [OSParameter(Description = "Number of documents to skip")] int skip, 
            [OSParameter(Description = "Limit of documents to return")] int limit);

        [OSAction(Description = "Updates documents in a MongoDB collection based on a filter")]
        ApiResponse UpdateDocument(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,  
            [OSParameter(Description = "JSON filter for documents to update. ex: { name: 'John' }")] string filterJson, 
            [OSParameter(Description = "JSON update definition. ex: { $set: { age: 31 } }")] string updateJson);

        [OSAction(Description = "Deletes documents from a MongoDB collection based on a filter")]
        ApiResponse DeleteDocument(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,  
            [OSParameter(Description = "JSON filter for documents to delete")] string filterJson);

        [OSAction(Description = "Returns the explain plan for an aggregation operation in MongoDB")]
        ApiResponse AggregateExplainer(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,  
            [OSParameter(Description = "Aggregation pipeline as JSON")] string aggregatePipeline, 
            [OSParameter(Description = "Verbose output flag")] bool verbose);

        [OSAction(Description = "Performs an aggregation query on a MongoDB collection")]
        ApiResponse AggregateCollection(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,  
            [OSParameter(Description = "Aggregation pipeline as JSON. Ex: [{ \"$match\": { \"status\": \"A\" } }, { \"$group\": { \"_id\": \"$category\", \"total\": { \"$sum\": \"$amount\" } } }]")] string aggregatePipeline);

        [OSAction(Description = "Retrieves statistics for a MongoDB collection")]
        ApiResponse GetCollectionStats(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config,  
            [OSParameter(Description = "Name of the collection to get stats for")] string collectionName);

        [OSAction(Description = "Retrieves index information for a MongoDB collection")]
        ApiResponse GetIndexInfo(
            [OSParameter(Description = "Mongo Configuration")] MongoConfig config);
    }
}

