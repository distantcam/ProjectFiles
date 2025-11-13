using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ProjectFilesGenerator;

public static class TestHelper
{
    public static Task Verify(
        string projectContent,
        string[] filePaths,
        [CallerMemberName] string testName = "")
    {
        // Create a temporary directory for the test
        var tempDir = Path.Combine(Path.GetTempPath(), "ProjectFilesGenerator.Tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Write the project file
            var projectPath = Path.Combine(tempDir, "TestProject.csproj");
            File.WriteAllText(projectPath, projectContent);

            // Create all the files referenced in the project
            foreach (var filePath in filePaths)
            {
                var normalizedPath = filePath.Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(tempDir, normalizedPath);
                var directory = Path.GetDirectoryName(fullPath);

                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(fullPath, $"Test content for {Path.GetFileName(fullPath)}");
            }

            // Set up the compilation
            var source = """
                namespace TestProject
                {
                    public class Program
                    {
                        public static void Main()
                        {
                        }
                    }
                }
                """;

            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            };

            var compilation = CSharpCompilation.Create(
                assemblyName: "TestProject",
                syntaxTrees: [syntaxTree],
                references: references,
                options: new(OutputKind.ConsoleApplication));

            // Create the source generator
            var generator = new ProjectFilesSourceGenerator();

            // Create additional text for the project file
            var additionalText = new TestAdditionalText(projectPath, projectContent);

            // Run the generator
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            driver = driver.AddAdditionalTexts([additionalText]);
            driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            // Get the results
            var runResult = driver.GetRunResult();

            // Verify using Verify.SourceGenerators
            return Verifier.Verify(runResult)
                .UseMethodName(testName);
        }
        finally
        {
            // Clean up temp directory
            try
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, recursive: true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    class TestAdditionalText(string path, string text) :
        AdditionalText
    {
        public override string Path { get; } = path;

        public override SourceText GetText(CancellationToken cancellationToken = default) =>
            SourceText.From(text);
    }
}
