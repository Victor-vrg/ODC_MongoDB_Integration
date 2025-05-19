
---

# 📘 **Documentação – Suporte a Transações MongoDB (vNext)**

## 🆕 **O que foi adicionado nesta versão**

### ✅ 1. **Gerenciador de Transações: `MongoTransactionManager`**

Nova classe utilitária criada em `Helpers` para controlar sessões MongoDB de forma stateless com cache interno e TTL.

**Funções disponíveis:**

* `GetOrCreateSession(...)`: Cria ou reutiliza sessão MongoDB.
* `CommitTransaction(...)`: Comita uma sessão e remove do cache.
* `AbortTransaction(...)`: Faz rollback de uma sessão e remove do cache.
* `CommitTransactionAction(...)`: Versão amigável para OutSystems.
* `AbortTransactionAction(...)`: Versão amigável para OutSystems.

---

### ✅ 2. **Suporte a `sessionId` nas funções CRUD**

* As funções podem receber um novo parâmetro opcional: `sessionId`
* Se presente, a função usa a sessão correspondente no cache
* Caso contrário, segue o fluxo de commit automático
* Depende também da flag `AutoCommitTransactions` em `MongoConfig`

**Exemplo:**

```csharp
public MongoDBConectorResponse CreateDocument(MongoConfig config, string documentJson, string? sessionId = null)
```

---

### ✅ 3. **Novas ações públicas expostas no `IMongoDB`**

Para controle explícito via OutSystems:

#### `CommitTransaction`

```csharp
[OSAction]
MongoDBConectorResponse CommitTransaction(MongoConfig config, string sessionId)
```

#### `AbortTransaction`

```csharp
[OSAction]
MongoDBConectorResponse AbortTransaction(MongoConfig config, string sessionId)
```

Essas ações usam os métodos `MongoTransactionManager.CommitTransactionAction` e `AbortTransactionAction`.

---

## 📌 Como utilizar no OutSystems

### 🚀 Modo com transação explícita:

1. Chame `CreateDocument(...)` passando um `sessionId`
2. Após todas as operações desejadas, chame `CommitTransaction(...)`
3. Em caso de erro, chame `AbortTransaction(...)`

### ⚙️ Modo padrão (automático):

1. Apenas chame `CreateDocument(...)` sem `sessionId`
2. Se `config.AutoCommitTransactions = true`, a operação é comitada automaticamente

---

## 💡 Dicas de uso

* A sessão tem um TTL padrão de **5 minutos**
* `sessionId` é uma `string GUID`
* Múltiplas funções podem compartilhar a mesma `sessionId`
* Após commit/abort, a sessão é automaticamente descartada
* Ideal para fluxos com múltiplas operações (transações compostas)

---


