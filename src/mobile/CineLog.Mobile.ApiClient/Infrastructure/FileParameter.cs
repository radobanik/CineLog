
namespace CineLog.Mobile.ApiClient.Infrastructure;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class FileParameter
{
    public FileParameter(System.IO.Stream data) : this(data, null, null)
    {
    }

    public FileParameter(System.IO.Stream data, string? fileName) : this(data, fileName, null)
    {
    }

    public FileParameter(System.IO.Stream data, string? fileName, string? contentType)
    {
        Data = data;
        FileName = fileName;
        ContentType = contentType;
    }

    public System.IO.Stream Data { get; private set; }
    public string? FileName { get; private set; }
    public string? ContentType { get; private set; }
}
