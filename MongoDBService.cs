using MongoDB.Driver;
using MongoDB.Bson;
using ODC_Mongo.helpers;
using MongoDB.Bson.Serialization;

namespace MongoDB_ODC
{
    public class MongoDBService : IMongoDB
    {
        public MongoDBConectorResponse CreateDocument(MongoConfig config, string documentJson)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var document = BsonDocument.Parse(documentJson);
                collection.InsertOne(document);
                return new MongoDBConectorResponse { Success = true, Message = "Document created successfully" };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse { Success = false, Message = $"Create failed: {ex.Message}" };
            }
        }

        public MongoDBConectorResponse GetDocuments(MongoConfig config, string filterJson)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var filter = string.IsNullOrEmpty(filterJson)
                    ? FilterDefinition<BsonDocument>.Empty
                    : new JsonFilterDefinition<BsonDocument>(filterJson);

                var documents = collection.Find(filter).ToList();
                return new MongoDBConectorResponse { Success = true, Data = JsonHelper.ConvertBsonToJson(documents) };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse { Success = false, Message = $"Query failed: {ex.Message}" };
            }
        }

        public MongoDBConectorResponse GetPagedDocuments(MongoConfig config, int skip, int limit)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);

                var documents = collection.Find(FilterDefinition<BsonDocument>.Empty)
                                          .Skip(skip)
                                          .Limit(limit)
                                          .ToList();

                return new MongoDBConectorResponse
                {
                    Success = true,
                    Data = JsonHelper.ConvertBsonToJson(documents)
                };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse
                {
                    Success = false,
                    Message = $"getPagedDocuments failed: {ex.Message}"
                };
            }
        }


        public MongoDBConectorResponse UpdateDocument(MongoConfig config, string filterJson, string updateJson)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<object>(config.CollectionName);
                var filter = new JsonFilterDefinition<object>(filterJson);
                var update = new JsonUpdateDefinition<object>(updateJson);

                var result = collection.UpdateOne(filter, update);
                return new MongoDBConectorResponse
                {
                    Success = result.IsAcknowledged,
                    Message = $"Modified {result.ModifiedCount} documents"
                };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse { Success = false, Message = $"Update failed: {ex.Message}" };
            }
        }

        public MongoDBConectorResponse DeleteDocument(MongoConfig config, string filterJson)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<object>(config.CollectionName);
                var filter = new JsonFilterDefinition<object>(filterJson);

                var result = collection.DeleteOne(filter);
                return new MongoDBConectorResponse
                {
                    Success = result.IsAcknowledged,
                    Message = $"Deleted {result.DeletedCount} documents"
                };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse { Success = false, Message = $"Delete failed: {ex.Message}" };
            }
        }


        public MongoDBConectorResponse AggregateExplainer(MongoConfig config, string aggregatePipeline, bool verbose)
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
                return new MongoDBConectorResponse
                {
                    Success = true,
                    Data = result.ToJson()
                };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse
                {
                    Success = false,
                    Message = $"Aggregate explain failed: {ex.Message}"
                };
            }
        }

        public MongoDBConectorResponse AggregateCollection(MongoConfig config, string aggregatePipeline)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var pipeline = BsonSerializer.Deserialize<BsonDocument[]>(aggregatePipeline);

                var results = collection.Aggregate<BsonDocument>(pipeline).ToList();
                return new MongoDBConectorResponse
                {
                    Success = true,
                    Data = results.ToJson()
                };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse
                {
                    Success = false,
                    Message = $"Aggregation failed: {ex.Message}"
                };
            }
        }

        public MongoDBConectorResponse GetCollectionStats(MongoConfig config)
        {
            try
            {
                var database = GetDatabase(config);
                var command = new BsonDocument { { "collStats", config.CollectionName } };
                var stats = database.RunCommand<BsonDocument>(command);

                return new MongoDBConectorResponse
                {
                    Success = true,
                    Data = stats.ToJson()
                };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse { Success = false, Message = $"GetCollectionStats failed: {ex.Message}" };
            }
        }

        public MongoDBConectorResponse GetIndexInfo(MongoConfig config)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var indexes = collection.Indexes.List().ToList();

                return new MongoDBConectorResponse
                {
                    Success = true,
                    Data = indexes.ToJson()
                };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse { Success = false, Message = $"GetIndexInfo failed: {ex.Message}" };
            }
        }

        public MongoDBConectorResponse GetDocumentById(MongoConfig config, string id)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var document = collection.Find(x => x["_id"] == id).FirstOrDefault();

                return new MongoDBConectorResponse
                {
                    Success = true,
                    Data = document.ToJson()
                };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse { Success = false, Message = $"GetDocumentById failed: {ex.Message}" };
            }
        }

        //   create function CountDocuments 
        public MongoDBConectorResponse CountDocuments(MongoConfig config, string filterJson, bool explain)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var filter = string.IsNullOrEmpty(filterJson)
                    ? FilterDefinition<BsonDocument>.Empty
                    : new JsonFilterDefinition<BsonDocument>(filterJson);

                var count = collection.CountDocuments(filter);
                return new MongoDBConectorResponse
                {
                    Success = true,
                    Data = explain ? count.ToJson() : count.ToString()
                };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse { Success = false, Message = $" CountDocuments failed: {ex.Message}" };
            }
        }

        public MongoDBConectorResponse IsDocumentExist(MongoConfig config, string filterJson)
        {
            try
            {
                var database = GetDatabase(config);
                var collection = database.GetCollection<BsonDocument>(config.CollectionName);
                var filter = new JsonFilterDefinition<BsonDocument>(filterJson);

                var document = collection.Find(filter).FirstOrDefault();
                return new MongoDBConectorResponse
                {
                    Success = document != null,
                    Data = document?.ToJson() ?? ""
                };
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse { Success = false, Message = $"IsDocumentExist failed: {ex.Message}" };
            }
        }

        /// <summary>
        /// Establishes a connection to a MongoDB database using the provided configuration settings.
        /// </summary>
        /// <param name="config">An instance of MongoConfig containing connection details such as
        /// connection string, database name, connection timeout, pool size, and SSL usage.</param>
        /// <returns>An IMongoDatabase instance representing the connected database.</returns>
        /// <remarks>
        /// This method sets various connection settings, including server selection timeout, 
        /// connection timeout, maximum connection pool size, and TLS usage. A new MongoClient 
        /// is created for each request, ensuring stateless operations.
        /// </remarks>

        private IMongoDatabase GetDatabase(MongoConfig config)
        {
            var settings = MongoClientSettings.FromConnectionString(config.ConnectionString);


            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(
        config.ConnectTimeout.HasValue && config.ConnectTimeout.Value > 0
            ? config.ConnectTimeout.Value
            : 60
    );
            settings.ConnectTimeout = settings.ServerSelectionTimeout;

            settings.MaxConnectionPoolSize = (config.MaxPoolSize.HasValue && config.MaxPoolSize.Value >= 1)
                ? config.MaxPoolSize.Value
        : 100;
            settings.UseTls = config.UseSSL ?? true;

            var client = new MongoClient(settings);
            return client.GetDatabase(config.DatabaseName);
        }

    }
}
