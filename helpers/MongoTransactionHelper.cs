// Classe auxiliar para gerenciar sessões de transação
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace MongoDB_ODC.Helpers
{
    public static class MongoTransactionManager
    {
        private static readonly ConcurrentDictionary<string, IClientSessionHandle> Sessions = new();
        private static readonly TimeSpan DefaultSessionTTL = TimeSpan.FromMinutes(5);
        private static readonly ConcurrentDictionary<string, DateTime> SessionExpirations = new();

        public static (IClientSessionHandle Session, string SessionId) GetOrCreateSession(MongoClient client, string? sessionId, int timeoutSeconds)
        {
            if (!string.IsNullOrEmpty(sessionId) && Sessions.TryGetValue(sessionId, out var existingSession))
            {
                // Verifica expiração
                if (SessionExpirations.TryGetValue(sessionId, out var expiresAt) && expiresAt > DateTime.UtcNow)
                {
                    return (existingSession, sessionId);
                }

                // Expirou
                RemoveSession(sessionId);
            }

            var session = client.StartSession();
            session.StartTransaction();

            var newSessionId = Guid.NewGuid().ToString();
            Sessions[newSessionId] = session;
            SessionExpirations[newSessionId] = DateTime.UtcNow.AddSeconds(timeoutSeconds);
            return (session, newSessionId);
        }

        public static bool CommitTransaction(string sessionId, out string message)
        {
            if (Sessions.TryRemove(sessionId, out var session))
            {
                SessionExpirations.TryRemove(sessionId, out _);
                session.CommitTransaction();
                session.Dispose();
                message = "Transação comitada.";
                return true;
            }

            message = "Sessão não encontrada ou já comitada.";
            return false;
        }

        public static bool AbortTransaction(string sessionId, out string message)
        {
            if (Sessions.TryRemove(sessionId, out var session))
            {
                SessionExpirations.TryRemove(sessionId, out _);
                session.AbortTransaction();
                session.Dispose();
                message = "Transação abortada.";
                return true;
            }

            message = "Sessão não encontrada ou já abortada.";
            return false;
        }

        private static void RemoveSession(string sessionId)
        {
            if (Sessions.TryRemove(sessionId, out var session))
            {
                session.Dispose();
                SessionExpirations.TryRemove(sessionId, out _);
            }
        }

         public static MongoDBConectorResponse CommitTransactionAction(string sessionId)
        {
            var config = new MongoDBConectorResponse();
            if (CommitTransaction(sessionId, out var message))
            {
                return new MongoDBConectorResponse(true, message);
            }
            return new MongoDBConectorResponse(false, message);
        }

        public static MongoDBConectorResponse AbortTransactionAction(string sessionId)
        {
            var config = new MongoDBConectorResponse();
            if (AbortTransaction(sessionId, out var message))
            {
                return new MongoDBConectorResponse(true, message);
            }
            return new MongoDBConectorResponse(false, message);
        }
    }
}
