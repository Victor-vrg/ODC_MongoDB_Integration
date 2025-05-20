// Classe auxiliar para gerenciar sessões de transação
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace MongoDB_ODC.Helpers
{
    public static class MongoTransactionManager
    {
        private static readonly ConcurrentDictionary<string, IClientSessionHandle> Sessions = new();
        private static readonly ConcurrentDictionary<string, DateTime> SessionExpirations = new();
        private static readonly TimeSpan DefaultSessionTTL = TimeSpan.FromMinutes(5);

        public static (IClientSessionHandle Session, string SessionId) GetOrCreateSession(MongoClient client, string? sessionId, int timeoutSeconds)
        {
            if (timeoutSeconds <= 0)
                timeoutSeconds = (int)DefaultSessionTTL.TotalSeconds;

            if (!string.IsNullOrEmpty(sessionId) && Sessions.TryGetValue(sessionId, out var existingSession))
            {
                if (SessionExpirations.TryGetValue(sessionId, out var expiresAt) && expiresAt > DateTime.UtcNow)
                {
                    return (existingSession, sessionId);
                }
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
                try
                {
                    session.CommitTransaction();
                    message = "Transação comitada.";
                }
                catch (Exception ex)
                {
                    message = $"Erro ao comitar: {ex.Message}";
                    return false;
                }

                try { session.Dispose(); } catch { /* Proteção contra Dispose duplo */ }
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
                try
                {
                    session.AbortTransaction();
                    message = "Transação abortada.";
                }
                catch (Exception ex)
                {
                    message = $"Erro ao abortar: {ex.Message}";
                    return false;
                }

                try { session.Dispose(); } catch { /* Proteção contra Dispose duplo */ }
                return true;
            }

            message = "Sessão não encontrada ou já abortada.";
            return false;
        }

        private static void RemoveSession(string sessionId)
        {
            if (Sessions.TryRemove(sessionId, out var session))
            {
                try { session.Dispose(); } catch { }
                SessionExpirations.TryRemove(sessionId, out _);
            }
        }

        public static MongoDBConectorResponse CommitTransactionAction(string sessionId)
        {
            if (CommitTransaction(sessionId, out var message))
            {
                return new MongoDBConectorResponse(true, message);
            }
            return new MongoDBConectorResponse(false, message);
        }

        public static MongoDBConectorResponse AbortTransactionAction(string sessionId)
        {
            if (AbortTransaction(sessionId, out var message))
            {
                return new MongoDBConectorResponse(true, message);
            }
            return new MongoDBConectorResponse(false, message);
        }
    }
}
