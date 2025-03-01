using MongoDB.Bson.IO;
using MongoDB.Bson;


namespace ODC_Mongo.helpers
{
    public static class JsonHelper
    {
        public static string ConvertBsonToJson(IEnumerable<BsonDocument> documents)
        {
            var jsonWriterSettings = new JsonWriterSettings
            {
                OutputMode = JsonOutputMode.CanonicalExtendedJson
            };
            return new BsonArray(documents).ToJson(jsonWriterSettings);
        }
    }
}
