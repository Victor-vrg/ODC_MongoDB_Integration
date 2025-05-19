# 📦 MongoDB Conector ODC – Changelog

##  2025-05-19

### 🚀 Adicionado
- Suporte completo a transações MongoDB com controle via `sessionId`.
- Nova estrutura de gerenciamento `MongoTransactionManager` com cache seguro de sessões.
- Expiração automática de sessões inativas após 5 minutos (TTL configurado internamente).
- Métodos auxiliares públicos:
  - `GetOrCreateSession`
  - `CommitTransaction`
  - `AbortTransaction`

### ➕ Novas ações expostas no conector (`IMongoDB.cs`)
- `CommitTransaction(MongoConfig config, string sessionId)`
- `AbortTransaction(MongoConfig config, string sessionId)`

### ✏️ Alterações em métodos existentes
- Todos os métodos CRUD (`CreateDocument`, `UpdateDocument`, etc.) agora suportam parâmetro opcional `sessionId`.
- Uso condicional de transações com base em:
  - `sessionId` fornecido
  - `AutoCommitTransactions = false` no `MongoConfig`

### 🧠 Observações
- Se `sessionId` não for fornecido, a operação será comitada automaticamente.
- O controle real de transações ocorre apenas quando `sessionId` é usado.
- SessionId é retornado nas respostas (`MongoDBConectorResponse`) quando aplicável.

---

