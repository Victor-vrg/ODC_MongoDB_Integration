# Roadmap de Uso do Conector MongoDB

---

### **1. Cenário de Sucesso: Transação Manual Completa (AutoCommit=false)**
```plantuml
@startuml
title "Cenário de Sucesso: Transação Manual Completa"

participant "Client" as client
participant "Server Action: CreateDocument" as create
participant "Server Action: UpdateDocument" as update
participant "MongoTransactionManager" as tx
participant "MongoDB" as db

client -> create: Executa CreateDocument
activate create
  create -> tx: GetOrCreateSession(sessionId=null)
  activate tx
  tx --> create: newSessionId="TX123"
  create -> db: Insert (session=TX123)\n**AutoCommit=false**
  db --> create: OK (pending)
  create --> client: Success ✅\nSessionId=TX123\ntransactionPending=true
deactivate create

client -> update: Executa UpdateDocument\n(sessionId=TX123)
activate update
  update -> tx: GetOrCreateSession(sessionId="TX123")
  tx --> update: Reusa TX123
  update -> db: Update (session=TX123)
  db --> update: OK (pending)
  update --> client: Success ✅\nSessionId=TX123
deactivate update

client -> "Server Action: CommitTransaction": Executa Commit\n(sessionId=TX123)
activate CommitTransaction
  CommitTransaction -> tx: CommitTransaction(TX123)
  tx -> db: Commit TX123
  db --> tx: OK
  tx --> CommitTransaction: Success ✅
  CommitTransaction --> client: Transação comitada!
deactivate CommitTransaction
@enduml
```

---

### **2. Cenário de Erro: AutoCommit Ativo com Tentativa de Commit Manual**
```plantuml
@startuml
title "Cenário de Erro: AutoCommit=true com Commit"

participant "Client" as client
participant "Server Action: CreateDocument" as create
participant "MongoTransactionManager" as tx
participant "MongoDB" as db

client -> create: Executa CreateDocument\n(AutoCommit=true)
activate create
  create -> db: Insert (auto-commit)\n**Sem sessionId**
  db --> create: OK (committed)
  create --> client: Success ✅\nSem sessionId
deactivate create

client -> "Server Action: CommitTransaction": Tenta Commit\n(sessionId=null)
activate CommitTransaction
  CommitTransaction -> tx: CommitTransaction(null)
  tx --> CommitTransaction: Erro: Sessão não existe ❌
  CommitTransaction --> client: Failure: "Sessão não encontrada"
deactivate CommitTransaction
@enduml
```

---

### **3. Cenário de Sucesso: AutoCommit=true (Comportamento Default)**
```plantuml
@startuml
title "Cenário de Sucesso: AutoCommit=true"

participant "Client" as client
participant "Server Action: CreateDocument" as create
participant "MongoDB" as db

client -> create: Executa CreateDocument\n(sessionId=null)
activate create
  create -> db: Insert (auto-commit)\n**Sem transação**
  db --> create: OK (committed)
  create --> client: Success ✅\nData válida
deactivate create
@enduml
```

---

### **4. Cenário de Erro: Transação Abandonada (Timeout)**
```plantuml
@startuml
title "Cenário de Erro: Transação Abandonada"

participant "Client" as client
participant "Server Action: CreateDocument" as create
participant "MongoTransactionManager" as tx
participant "MongoDB" as db

client -> create: Executa CreateDocument\n(AutoCommit=false)
activate create
  create -> tx: GetOrCreateSession()
  tx --> create: newSessionId="TX456"\nExpira em 5m
  create -> db: Insert (session=TX456)\n**pending**
  create --> client: Success ✅\nSessionId=TX456
deactivate create

... Espera 6 minutos ...

client -> "Server Action: CommitTransaction": Tenta Commit(TX456)
activate CommitTransaction
  CommitTransaction -> tx: CommitTransaction(TX456)
  tx --> CommitTransaction: Erro: Sessão expirada ❌
  CommitTransaction --> client: Failure: "Sessão não encontrada"
deactivate CommitTransaction
@enduml
```

---
