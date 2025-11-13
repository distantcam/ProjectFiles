using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ProjectFilesGenerator;

[Generator]
[SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1035:Do not use APIs banned for analyzers")]
public class ProjectFilesSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all additional files that are .csproj files
        var projectFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase));

        // Parse the project file and extract file paths
        var filePaths = projectFiles
            .Select((file, cancellationToken) =>
            {
                var text = file.GetText(cancellationToken);
                if (text == null)
                    return ImmutableArray<string>.Empty;

                var projectDir = Path.GetDirectoryName(file.Path) ?? string.Empty;
                return ParseProjectFile(text.ToString(), projectDir);
            })
            .Where(files => files.Length > 0);

        // Generate the source
        context.RegisterSourceOutput(filePaths, (spc, files) =>
        {
            var source = GenerateSource(files);
            spc.AddSource("ProjectFiles.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }

    private static ImmutableArray<string> ParseProjectFile(string content, string projectDir)
    {
        try
        {
            var doc = XDocument.Parse(content);
            var ns = doc.Root?.Name.Namespace ?? XNamespace.None;

            var files = new List<string>();

            // Find all None, Content, and other item types with CopyToOutputDirectory
            var itemGroups = doc.Descendants()
                .Where(e => e.Name.LocalName == "ItemGroup");

            foreach (var itemGroup in itemGroups)
            {
                foreach (var item in itemGroup.Elements())
                {
                    var copyToOutput = item.Elements()
                        .FirstOrDefault(e => e.Name.LocalName == "CopyToOutputDirectory");

                    if (copyToOutput?.Value == "PreserveNewest" || copyToOutput?.Value == "Always")
                    {
                        var includeAttr = item.Attribute("Include")?.Value ?? item.Attribute("Update")?.Value;
                        if (!string.IsNullOrEmpty(includeAttr))
                        {
                            // Expand glob patterns
                            var expandedFiles = ExpandGlobPattern(includeAttr!, projectDir);
                            files.AddRange(expandedFiles);
                        }
                    }
                }
            }

            return files.Distinct().OrderBy(f => f).ToImmutableArray();
        }
        catch
        {
            return ImmutableArray<string>.Empty;
        }
    }

    private static IEnumerable<string> ExpandGlobPattern(string pattern, string projectDir)
    {
        // Normalize path separators
        pattern = pattern.Replace('/', Path.DirectorySeparatorChar);

        if (!Directory.Exists(projectDir))
            return Enumerable.Empty<string>();

        // Check if pattern contains wildcards
        if (pattern.Contains("*") || pattern.Contains("?"))
        {
            var parts = pattern.Split(Path.DirectorySeparatorChar);
            var hasRecursive = parts.Contains("**");

            if (hasRecursive)
            {
                // Handle ** recursive pattern
                var beforeRecursive = string.Join(Path.DirectorySeparatorChar.ToString(),
                    parts.TakeWhile(p => p != "**"));
                var afterRecursive = string.Join(Path.DirectorySeparatorChar.ToString(),
                    parts.SkipWhile(p => p != "**").Skip(1));

                var searchDir = string.IsNullOrEmpty(beforeRecursive)
                    ? projectDir
                    : Path.Combine(projectDir, beforeRecursive);

                if (!Directory.Exists(searchDir))
                    return Enumerable.Empty<string>();

                var searchPattern = string.IsNullOrEmpty(afterRecursive) ? "*.*" : afterRecursive;

                try
                {
                    var foundFiles = Directory.GetFiles(searchDir, searchPattern, SearchOption.AllDirectories);
                    return foundFiles.Select(f => GetRelativePath(projectDir, f));
                }
                catch
                {
                    return Enumerable.Empty<string>();
                }
            }
            else
            {
                // Handle single directory with wildcards
                var dirPart = Path.GetDirectoryName(pattern) ?? string.Empty;
                var filePart = Path.GetFileName(pattern);

                var searchDir = string.IsNullOrEmpty(dirPart)
                    ? projectDir
                    : Path.Combine(projectDir, dirPart);

                if (!Directory.Exists(searchDir))
                    return Enumerable.Empty<string>();

                try
                {
                    var foundFiles = Directory.GetFiles(searchDir, filePart);
                    return foundFiles.Select(f => GetRelativePath(projectDir, f));
                }
                catch
                {
                    return Enumerable.Empty<string>();
                }
            }
        }
        else
        {
            // No wildcards - just return the file if it exists
            var fullPath = Path.Combine(projectDir, pattern);
            return File.Exists(fullPath) ? [pattern] : Enumerable.Empty<string>();
        }
    }

    private static string GetRelativePath(string basePath, string fullPath)
    {
        var baseUri = new Uri(EnsureTrailingSlash(basePath));
        var fullUri = new Uri(fullPath);
        var relativeUri = baseUri.MakeRelativeUri(fullUri);
        return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
    }

    private static string EnsureTrailingSlash(string path)
    {
        if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            return path + Path.DirectorySeparatorChar;
        return path;
    }

    private static string GenerateSource(ImmutableArray<string> files)
    {
        var tree = BuildFileTree(files);
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("namespace ProjectFiles;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Provides strongly-typed access to project files marked with CopyToOutputDirectory.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static partial class ProjectFiles");
        sb.AppendLine("{");

        GenerateTreeNode(sb, tree, 1);

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static FileTreeNode BuildFileTree(ImmutableArray<string> files)
    {
        var root = new FileTreeNode { Name = "Root", IsDirectory = true };

        foreach (var file in files)
        {
            var parts = file.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var current = root;

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                var isLast = i == parts.Length - 1;

                if (!current.Children.TryGetValue(part, out var child))
                {
                    child = new()
                    {
                        Name = part,
                        IsDirectory = !isLast,
                        FullPath = isLast ? file : null
                    };
                    current.Children[part] = child;
                }

                current = child;
            }
        }

        return root;
    }

    private static void GenerateTreeNode(StringBuilder sb, FileTreeNode node, int indent)
    {
        var indentStr = new string(' ', indent * 4);

        foreach (var child in node.Children.OrderBy(kvp => kvp.Key))
        {
            var name = child.Key;
            var childNode = child.Value;

            if (childNode.IsDirectory)
            {
                // Generate nested static class for directory
                var className = ToValidIdentifier(name);
                sb.AppendLine($"{indentStr}/// <summary>");
                sb.AppendLine($"{indentStr}/// Files in the '{name}' directory.");
                sb.AppendLine($"{indentStr}/// </summary>");
                sb.AppendLine($"{indentStr}public static partial class {className}");
                sb.AppendLine($"{indentStr}{{");

                GenerateTreeNode(sb, childNode, indent + 1);

                sb.AppendLine($"{indentStr}}}");
                sb.AppendLine();
            }
            else
            {
                // Generate property for file
                var propertyName = ToValidIdentifier(Path.GetFileNameWithoutExtension(name));
                var extension = Path.GetExtension(name);
                if (!string.IsNullOrEmpty(extension))
                {
                    propertyName += ToValidIdentifier(extension.TrimStart('.'), true);
                }

                var path = childNode.FullPath!.Replace("\\", "\\\\");

                sb.AppendLine($"{indentStr}/// <summary>");
                sb.AppendLine($"{indentStr}/// Path to '{name}'.");
                sb.AppendLine($"{indentStr}/// </summary>");
                sb.AppendLine($"{indentStr}public static string {propertyName} => \"{path}\";");
                sb.AppendLine();
            }
        }
    }

    private static string ToValidIdentifier(string name, bool capitalizeFirst = false)
    {
        if (string.IsNullOrEmpty(name))
            return "_";

        var sb = new StringBuilder();
        var capitalizeNext = capitalizeFirst;

        foreach (var ch in name)
        {
            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(capitalizeNext ? char.ToUpperInvariant(ch) : ch);
                capitalizeNext = false;
            }
            else if (ch == '_')
            {
                sb.Append('_');
                capitalizeNext = false;
            }
            else
            {
                // Replace invalid characters with underscore and capitalize next
                if (sb.Length > 0 && sb[sb.Length - 1] != '_')
                {
                    capitalizeNext = true;
                }
            }
        }

        var result = sb.ToString();

        // Ensure it starts with a letter or underscore
        if (result.Length > 0 && char.IsDigit(result[0]))
            result = "_" + result;

        // Handle C# keywords
        if (IsCSharpKeyword(result))
            result = "@" + result;

        // Capitalize first letter if it's a class/namespace
        if (!capitalizeFirst && result.Length > 0)
        {
            result = char.ToUpperInvariant(result[0]) + result.Substring(1);
        }

        return string.IsNullOrEmpty(result) ? "_" : result;
    }

    private static bool IsCSharpKeyword(string identifier)
    {
        var keywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw",
            "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
            "virtual", "void", "volatile", "while"
        };

        return keywords.Contains(identifier);
    }

    private class FileTreeNode
    {
        public string Name { get; set; } = string.Empty;
        public bool IsDirectory { get; set; }
        public string? FullPath { get; set; }
        public Dictionary<string, FileTreeNode> Children { get; } = [];
    }
}
