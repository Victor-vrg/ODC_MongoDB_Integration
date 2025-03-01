# Exemplo de structure

```csharp
using System;
using OutSystems.ExternalLibraries.SDK;

namespace SuaNamespace
{
    /// <summary>
    /// Descrição clara da finalidade da estrutura
    /// </summary>
    [OSStructure(
        Description = "Exemplo de estrutura para integração com OutSystems",
        IconResourceName = "SuaNamespace.Resources.example_icon.png" // Opcional
    )]
    public struct ExemploStruct
    {
        // 1. Propriedades sempre em PascalCase com get/set
        [OSStructureField(
            DataType = OSDataType.Text,
            Description = "Campo obrigatório com descrição detalhada",
            IsMandatory = true
        )]
        public string CampoObrigatorio { get; set; }

        // 2. Campos opcionais usam tipos anuláveis
        [OSStructureField(
            DataType = OSDataType.Integer,
            Description = "Campo numérico opcional com valor padrão",
            IsMandatory = false
        )]
        public int? CampoOpcional { get; set; }

        // 3. Tratamento especial para datas
        [OSStructureField(
            DataType = OSDataType.DateTime,
            Description = "Data no formato ISO 8601",
            IsMandatory = false
        )]
        public DateTime? DataISO { get; set; }

        // 4. Construtor com validação
        public ExemploStruct(
            string campoObrigatorio,
            int? campoOpcional = null,
            DateTime? dataISO = null)
        {
            // Validação de campos obrigatórios
            CampoObrigatorio = campoObrigatorio ?? throw new ArgumentNullException(nameof(campoObrigatorio));
            
            // Valores padrão para opcionais
            CampoOpcional = campoOpcional ?? 0;
            DataISO = dataISO?.ToUniversalTime() ?? DateTime.UtcNow;
        }

        // 5. Construtor privado para uso do OutSystems
        private ExemploStruct() 
        {
            // Necessário para serialização do OutSystems
            CampoObrigatorio = string.Empty;
            CampoOpcional = null;
            DataISO = null;
        }
    }
}
```
