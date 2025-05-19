using MongoDB.Driver;
using MongoDB.Bson;
using ODC_Mongo.helpers;
using MongoDB.Bson.Serialization;
using MongoDB_Integration.structures;
using MongoDB_ODC.Helpers;

namespace MongoDB_ODC
{
    public class MongoDBService : IMongoDB
    {

        /// <summary>
        /// Commit transaction
        /// </summary>
        /// <param name="config">MongoDB configuration</param>
        /// <param name="sessionId">Session id</param>
        /// <returns>MongoDB response</returns>
        public MongoDBConectorResponse MongoDBCommitTransaction(MongoConfig config, string sessionId)
        {
            return MongoTransactionManager.CommitTransactionAction(sessionId);
        }

        public MongoDBConectorResponse MongoDBAbortTransaction(MongoConfig config, string sessionId)
        {
            return MongoTransactionManager.AbortTransactionAction(sessionId);
        }


        public MongoDBConectorResponse CreateDocument(MongoConfig config, string documentJson, string? sessionId = null)
        {
            try
            {
                var client = new MongoClient(config.ConnectionString);
                var db = client.GetDatabase(config.DatabaseName);
                var collection = db.GetCollection<BsonDocument>(config.CollectionName);
                var document = BsonDocument.Parse(documentJson);

                IClientSessionHandle? session = null;
                string newSessionId = string.Empty;

                if (!config.AutoCommitTransactions || !string.IsNullOrEmpty(sessionId))
                {
                    (session, newSessionId) = MongoTransactionManager.GetOrCreateSession(client, sessionId, config.TransactionDefaultTimeout);
                    collection.InsertOne(session, document);
                    return new MongoDBConectorResponse(true, "Documento criado com transação pendente", null, newSessionId, true);
                }
                else
                {
                    collection.InsertOne(document);
                    return new MongoDBConectorResponse(true, "Documento criado com commit automático");
                }
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse(false, $"Erro: {ex.Message}");
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


        public MongoDBConectorResponse UpdateDocument(MongoConfig config, string filterJson, string updateJson, string? sessionId = null)
        {
            try
            {
                var client = new MongoClient(config.ConnectionString);
                var db = client.GetDatabase(config.DatabaseName);
                var collection = db.GetCollection<BsonDocument>(config.CollectionName); // Alterado para BsonDocument
                var filter = new JsonFilterDefinition<BsonDocument>(filterJson);
                var update = new JsonUpdateDefinition<BsonDocument>(updateJson);

                IClientSessionHandle? session = null;
                string newSessionId = string.Empty;

                // Lógica de transação
                if (!config.AutoCommitTransactions || !string.IsNullOrEmpty(sessionId))
                {
                    (session, newSessionId) = MongoTransactionManager.GetOrCreateSession(client, sessionId, config.TransactionDefaultTimeout);
                    var result = collection.UpdateOne(session, filter, update); // Passa a sessão
                    return new MongoDBConectorResponse(
                        success: result.IsAcknowledged,
                        message: $"Modified {result.ModifiedCount} documents",
                        data: null,
                        sessionId: newSessionId,
                        transactionPending: true
                    );
                }
                else
                {
                    var result = collection.UpdateOne(filter, update);
                    return new MongoDBConectorResponse(
                        success: result.IsAcknowledged,
                        message: $"Modified {result.ModifiedCount} documents (auto-committed)"
                    );
                }
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse(false, $"Update failed: {ex.Message}");
            }
        }
        public MongoDBConectorResponse DeleteDocument(MongoConfig config, string filterJson, string? sessionId = null)
        {
            try
            {
                var client = new MongoClient(config.ConnectionString);
                var db = client.GetDatabase(config.DatabaseName);
                var collection = db.GetCollection<BsonDocument>(config.CollectionName);
                var filter = new JsonFilterDefinition<BsonDocument>(filterJson);

                IClientSessionHandle? session = null;
                string newSessionId = string.Empty;

                if (!config.AutoCommitTransactions || !string.IsNullOrEmpty(sessionId))
                {
                    (session, newSessionId) = MongoTransactionManager.GetOrCreateSession(client, sessionId, config.TransactionDefaultTimeout);
                    var result = collection.DeleteOne(session, filter);
                    return new MongoDBConectorResponse(
                        success: result.IsAcknowledged,
                        message: $"Deleted {result.DeletedCount} documents",
                        sessionId: newSessionId,
                        transactionPending: true
                    );
                }
                else
                {
                    var result = collection.DeleteOne(filter);
                    return new MongoDBConectorResponse(
                        success: result.IsAcknowledged,
                        message: $"Deleted {result.DeletedCount} documents (auto-committed)"
                    );
                }
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse(false, $"Delete failed: {ex.Message}");
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

        /// <summary>
        /// Executes an aggregation pipeline on a collection.
        /// 
        /// If <paramref name="sessionId"/> is not null, uses a session for the aggregation.
        /// If <see cref="MongoConfig.AutoCommitTransactions"/> is false, sessions are used for aggregation.
        /// </summary>
        /// <param name="config">The MongoDB configuration.</param>
        /// <param name="aggregatePipeline">The aggregation pipeline as a string.</param>
        /// <param name="sessionId">The session identifier to use, or null for auto-commit.</param>
        /// <returns>A <see cref="MongoDBConectorResponse"/> with the result of the aggregation.</returns>
        public MongoDBConectorResponse AggregateCollection(
     MongoConfig config,
     string aggregatePipeline,
     string? sessionId = null // Adicionado parâmetro opcional
 )
        {
            try
            {
                var client = new MongoClient(config.ConnectionString);
                var db = client.GetDatabase(config.DatabaseName);
                var collection = db.GetCollection<BsonDocument>(config.CollectionName);
                var pipeline = BsonSerializer.Deserialize<BsonDocument[]>(aggregatePipeline);

                IClientSessionHandle? session = null;
                string newSessionId = string.Empty;

                // Lógica de transação (opcional, mesmo para leitura)
                if (!config.AutoCommitTransactions || !string.IsNullOrEmpty(sessionId))
                {
                    (session, newSessionId) = MongoTransactionManager.GetOrCreateSession(client, sessionId, config.TransactionDefaultTimeout);
                    var results = collection.Aggregate<BsonDocument>(session, pipeline).ToList(); // Passa a sessão
                    return new MongoDBConectorResponse(
                        success: true,
                        message: "Aggregation successful",
                        data: results.ToJson(),
                        sessionId: newSessionId,
                        transactionPending: true
                    );
                }
                else
                {
                    var results = collection.Aggregate<BsonDocument>(pipeline).ToList();
                    return new MongoDBConectorResponse(
                        success: true,
                        message: "Aggregation successful",
                        data: results.ToJson()
                    );
                }
            }
            catch (Exception ex)
            {
                return new MongoDBConectorResponse(
                    success: false,
                    message: $"Aggregation failed: {ex.Message}"
                );
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
