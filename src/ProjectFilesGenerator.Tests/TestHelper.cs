using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ProjectFilesGenerator;
using VerifyTests;
using VerifyXunit;

namespace ProjectFilesGenerator.Tests;

public static class TestHelper
{
    static TestHelper()
    {
        // Configure Verify for source generator testing
        VerifySourceGenerators.Initialize();
        
        // Customize settings
        VerifierSettings.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) => 
                new PathInfo(
                    directory: Path.Combine(projectDirectory, "Snapshots"),
                    typeName: type.Name,
                    methodName: method.Name));
    }

    public static Task Verify(
        string projectContent,
        params string[] filePaths,
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
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            // Create the source generator
            var generator = new ProjectFilesSourceGenerator();

            // Create additional text for the project file
            var additionalText = new TestAdditionalText(projectPath, projectContent);

            // Run the generator
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            driver = driver.AddAdditionalTexts(ImmutableArray.Create<AdditionalText>(additionalText));
            driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            // Get the results
            var runResult = driver.GetRunResult();

            // Verify using Verify.SourceGenerators
            return Verifier.Verify(runResult)
                .UseDirectory("Snapshots")
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

    private class TestAdditionalText : AdditionalText
    {
        private readonly string _text;

        public TestAdditionalText(string path, string text)
        {
            Path = path;
            _text = text;
        }

        public override string Path { get; }

        public override SourceText? GetText(CancellationToken cancellationToken = default)
        {
            return SourceText.From(_text);
        }
    }
}
