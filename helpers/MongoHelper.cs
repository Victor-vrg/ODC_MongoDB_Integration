using MongoDB.Bson;
using MongoDB.Driver;
using System;


namespace MongoDB_ODC.helpers
{
    public static class MongoHelper
    {
        public static BsonDocument ParseDocument(string json)
        {
            try
            {
                return BsonDocument.Parse(json);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid document JSON", ex);
            }
        }

        public static FilterDefinition<BsonDocument> ParseFilter(string json)
        {
            try
            {
                return BsonDocument.Parse(json);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid filter JSON", ex);
            }
        }

        public static UpdateDefinition<BsonDocument> ParseUpdate(string json)
        {
            try
            {
                return BsonDocument.Parse(json);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid update JSON", ex);
            }
        }

        public static MongoDBConectorResponse HandleException(Exception ex, string operation)
        {
            Console.WriteLine($"{operation} error: {ex.Message}");
            return new MongoDBConectorResponse(
                success: false,
                message: $"{operation} failed: {ex.Message}",
                data: null
            );
        }
    }
}
