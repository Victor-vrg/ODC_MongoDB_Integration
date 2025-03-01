using MongoDB.Driver;
using MongoDB.Bson;
using ODC_Mongo.helpers;
using MongoDB.Bson.Serialization;

namespace MongoDB_ODC
{
    public class MongoDBService : IMongoDB
    {
        public ApiResponse CreateDocument(MongoConfig config, string documentJson)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var document = BsonDocument.Parse(documentJson);
                collection.InsertOne(document);
                return new ApiResponse { Success = true, Message = "Document created successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"Create failed: {ex.Message}" };
            }
        }

        public ApiResponse GetDocuments(MongoConfig config, string filterJson)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var filter = string.IsNullOrEmpty(filterJson) 
                    ? FilterDefinition<BsonDocument>.Empty 
                    : new JsonFilterDefinition<BsonDocument>(filterJson);
                
                var documents = collection.Find(filter).ToList();
                return new ApiResponse { Success = true, Data = JsonHelper.ConvertBsonToJson(documents) };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"Query failed: {ex.Message}" };
            }
        }

        public ApiResponse GetPagedDocuments(MongoConfig config, int skip, int limit)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);

                var documents = collection.Find(FilterDefinition<BsonDocument>.Empty)
                                          .Skip(skip)
                                          .Limit(limit)
                                          .ToList();

                return new ApiResponse
                {
                    Success = true,
                    Data = JsonHelper.ConvertBsonToJson(documents)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = $"getPagedDocuments failed: {ex.Message}"
                };
            }
        }


        public ApiResponse UpdateDocument(MongoConfig config, string filterJson, string updateJson)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<object>(config.CollectionName);
                var filter = new JsonFilterDefinition<object>(filterJson);
                var update = new JsonUpdateDefinition<object>(updateJson);
                
                var result = collection.UpdateOne(filter, update);
                return new ApiResponse { 
                    Success = result.IsAcknowledged, 
                    Message = $"Modified {result.ModifiedCount} documents" 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"Update failed: {ex.Message}" };
            }
        }

        public ApiResponse DeleteDocument(MongoConfig config, string filterJson)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<object>(config.CollectionName);
                var filter = new JsonFilterDefinition<object>(filterJson);
                
                var result = collection.DeleteOne(filter);
                return new ApiResponse { 
                    Success = result.IsAcknowledged, 
                    Message = $"Deleted {result.DeletedCount} documents" 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"Delete failed: {ex.Message}" };
            }
        }


        public ApiResponse AggregateExplainer(MongoConfig config, string aggregatePipeline, bool verbose)
        {
            try
            {
                var database = GetDatabase(config);
                var command = new BsonDocument
        {
            { "explain", new BsonDocument
                {
                    { "aggregate", config.CollectionName },
                    { "pipeline", BsonSerializer.Deserialize<BsonArray>(aggregatePipeline) },
                    { "cursor", new BsonDocument() }
                }
            },
            { "verbosity", verbose ? "executionStats" : "queryPlanner" }
        };

                var result = database.RunCommand<BsonDocument>(command);
                return new ApiResponse
                {
                    Success = true,
                    Data = result.ToJson()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = $"Aggregate explain failed: {ex.Message}"
                };
            }
        }

        public ApiResponse AggregateCollection(MongoConfig config, string aggregatePipeline)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var pipeline = BsonSerializer.Deserialize<BsonDocument[]>(aggregatePipeline);

                var results = collection.Aggregate<BsonDocument>(pipeline).ToList();
                return new ApiResponse
                {
                    Success = true,
                    Data = results.ToJson()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = $"Aggregation failed: {ex.Message}"
                };
            }
        }

        public ApiResponse GetCollectionStats(MongoConfig config, string collectionName)
        {
            try
            {
                var database = GetDatabase(config);
                var command = new BsonDocument { { "collStats", collectionName } };
                var stats = database.RunCommand<BsonDocument>(command);
                
                return new ApiResponse { 
                    Success = true, 
                    Data = stats.ToJson() 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"GetCollectionStats failed: {ex.Message}" };
            }
        }

        public ApiResponse GetIndexInfo(MongoConfig config)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var indexes = collection.Indexes.List().ToList();
                
                return new ApiResponse { 
                    Success = true, 
                    Data = indexes.ToJson() 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"GetIndexInfo failed: {ex.Message}" };
            }
        }

        private IMongoDatabase GetDatabase(MongoConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            return client.GetDatabase(config.DatabaseName);
        }
    }
}
