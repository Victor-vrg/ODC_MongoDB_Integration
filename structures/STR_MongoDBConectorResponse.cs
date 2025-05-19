using OutSystems.ExternalLibraries.SDK;

namespace MongoDB_ODC
{
    [OSStructure(Description = "Resposta padrão da API")]
    public struct MongoDBConectorResponse
    {
        [OSStructureField(
            DataType = OSDataType.Boolean,
            Description = "Indica se a operação foi bem-sucedida",
            IsMandatory = true
        )]
        public bool Success;

        [OSStructureField(
            DataType = OSDataType.Text,
            Description = "Mensagem descritiva do resultado",
            IsMandatory = true
        )]
        public string Message;

        [OSStructureField(
            DataType = OSDataType.Text,
            Description = "Dados retornados (JSON)",
            IsMandatory = false
        )]
        public string Data { get; set; }

        [OSStructureField(
            DataType = OSDataType.Text,
            Description = "ID da sessão de transação ativa",
            IsMandatory = false
        )]
        public string SessionId { get; set; }

        [OSStructureField(
            DataType = OSDataType.Boolean,
            Description = "Indica se há transação pendente",
            IsMandatory = false
        )]
        public bool TransactionPending { get; set; }

        public MongoDBConectorResponse(bool success, string message, string? data = null, string? sessionId = null, bool transactionPending = false)
        {
            Success = success;
            Message = message;
            Data = data ?? string.Empty;
            SessionId = sessionId ?? string.Empty;
            TransactionPending = transactionPending;
        }
    }
}
