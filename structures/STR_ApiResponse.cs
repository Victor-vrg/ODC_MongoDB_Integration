using OutSystems.ExternalLibraries.SDK;

namespace MongoDB_ODC
{
    [OSStructure(Description = "Resposta padrão da API")]
    public struct ApiResponse
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
        public string Data { get; set; } // Propriedade com get/set
    // esse padrão e melhor para lidar no outsytems recebendo o objeto via string para deserializar lá
   public ApiResponse(bool success, string message, string? data = null)
{
    Success = success;
    Message = message;
    Data = data ?? string.Empty; // Evita null para compatibilidade
}
    }
}

