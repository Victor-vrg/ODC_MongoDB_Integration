# exemplo de interface

```csharp
using OutSystems.ExternalLibraries.SDK;


namespace SuaNamespace
{
    [OSInterface(Description = "Interface lorem", IconResourceName = "SuaNamespace.resources.teste.ico")]
    public interface Ilorem
    {
    
        CustomStucture CreateDocument(
        [OSParameter(Description = "Lorem Configuration")]

        ExampleConfig config,  

        [OSParameter(Description = "Json document")]

        string documentJson);
    }
}
```
