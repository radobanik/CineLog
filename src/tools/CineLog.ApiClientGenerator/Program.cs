using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration.CSharp;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddCommandLine(args)
    .Build();

var swaggerUrl = args.Length > 0 && !args[0].StartsWith("--") ? args[0] : config["SwaggerUrl"] ?? "http://localhost:5098/swagger/v1/swagger.json";
var outputDir = args.Length > 1 && !args[1].StartsWith("--") ? args[1] : config["OutputDir"] ?? Path.Combine(Directory.GetCurrentDirectory(), "GeneratedClients");
var clientNamespace = args.Length > 2 && !args[2].StartsWith("--") ? args[2] : config["Namespace"] ?? "CineLog.ApiClient";

if (!Path.IsPathRooted(outputDir))
    outputDir = Path.Combine(Directory.GetCurrentDirectory(), outputDir);

Console.WriteLine($"Fetching OpenAPI spec from {swaggerUrl}...");

OpenApiDocument document;
try
{
    document = await OpenApiDocument.FromUrlAsync(swaggerUrl);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to fetch OpenAPI spec: {ex.Message}");
    Console.Error.WriteLine("Make sure the API is running.");
    Environment.Exit(1);
    return;
}

Console.WriteLine($"Spec loaded: {document.Info.Title} v{document.Info.Version}");
Console.WriteLine($"Namespace   : {clientNamespace}");
Console.WriteLine($"Output      : {outputDir}");
Console.WriteLine();

Directory.CreateDirectory(outputDir);
foreach (var subfolder in new[] { "Clients", "Models", "Infrastructure" })
{
    var sub = Path.Combine(outputDir, subfolder);
    if (Directory.Exists(sub))
        Directory.Delete(sub, recursive: true);
}

var settings = new CSharpClientGeneratorSettings
{
    ClassName = "{controller}Client",
    OperationNameGenerator = new NSwag.CodeGeneration.OperationNameGenerators.MultipleClientsFromFirstTagAndOperationNameGenerator(),
    GenerateClientClasses = true,
    GenerateClientInterfaces = true,
    GenerateDtoTypes = true,
    UseBaseUrl = false,
    InjectHttpClient = true,
    GenerateExceptionClasses = true,
    ExceptionClass = "ApiException",
    WrapResponses = false,
    GenerateResponseClasses = false,
    CSharpGeneratorSettings =
    {
        Namespace = clientNamespace,
        GenerateDefaultValues = true,
        GenerateDataAnnotations = false,
        ClassStyle = CSharpClassStyle.Poco,
        GenerateNullableReferenceTypes = true,
        GenerateOptionalPropertiesAsNullable = true,
    }
};

var generator = new CSharpClientGenerator(document, settings);
Console.WriteLine("Generating code...");
var fullCode = generator.GenerateFile();

Console.WriteLine("Splitting into individual files...");
var syntaxTree = CSharpSyntaxTree.ParseText(fullCode);
var root = (CompilationUnitSyntax)await syntaxTree.GetRootAsync();

var usings = root.Usings;

string namespaceName = clientNamespace;
IEnumerable<MemberDeclarationSyntax> typeMembers;

var fileScopedNs = root.Members.OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
var blockNs = root.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

if (fileScopedNs != null)
{
    namespaceName = fileScopedNs.Name.ToString();
    typeMembers = fileScopedNs.Members;
}
else if (blockNs != null)
{
    namespaceName = blockNs.Name.ToString();
    typeMembers = blockNs.Members;
}
else
{
    typeMembers = root.Members;
}

int fileCount = 0;

foreach (var member in typeMembers)
{
    string? typeName = member switch
    {
        ClassDeclarationSyntax c => c.TypeParameterList?.Parameters.Count > 0
                                            ? $"{c.Identifier.Text}`{c.TypeParameterList.Parameters.Count}"
                                            : c.Identifier.Text,
        InterfaceDeclarationSyntax i => i.TypeParameterList?.Parameters.Count > 0
                                            ? $"{i.Identifier.Text}`{i.TypeParameterList.Parameters.Count}"
                                            : i.Identifier.Text,
        EnumDeclarationSyntax e => e.Identifier.Text,
        RecordDeclarationSyntax r => r.TypeParameterList?.Parameters.Count > 0
                                            ? $"{r.Identifier.Text}`{r.TypeParameterList.Parameters.Count}"
                                            : r.Identifier.Text,
        _ => null
    };

    if (typeName is null) continue;

    string subfolder = ClassifyType(typeName);
    var subDir = Path.Combine(outputDir, subfolder);
    Directory.CreateDirectory(subDir);

    var sb = new StringBuilder();
    foreach (var u in usings)
        sb.AppendLine(u.ToFullString().TrimEnd());
    if (subfolder == "Clients")
    {
        sb.AppendLine($"using {namespaceName}.Models;");
        sb.AppendLine($"using {namespaceName}.Infrastructure;");
    }
    sb.AppendLine();
    sb.AppendLine($"namespace {namespaceName}.{subfolder};");
    sb.AppendLine();
    sb.AppendLine(member.NormalizeWhitespace().ToFullString());

    var filePath = Path.Combine(subDir, $"{typeName}.cs");
    await File.WriteAllTextAsync(filePath, sb.ToString());

    Console.WriteLine($"  {subfolder}/{typeName}.cs");
    fileCount++;
}

Console.WriteLine();
Console.WriteLine($"Generated {fileCount} files in {outputDir}");

static string ClassifyType(string name)
{
    var baseName = name.Contains('`') ? name[..name.IndexOf('`')] : name;

    if (baseName.Length > 1 && baseName[0] == 'I' && char.IsUpper(baseName[1]) && baseName.EndsWith("Client"))
        return "Clients";

    if (baseName.EndsWith("Client"))
        return "Clients";

    if (baseName is "ApiException" or "FileResponse" or "FileParameter" or "JsonExceptionConverter")
        return "Infrastructure";

    return "Models";
}
