using System;
using System.IO;
using ProjectFiles;

// Example usage of the generated ProjectFiles API
Console.WriteLine("=== Project Files Demo ===");
Console.WriteLine();

// Access files with strongly-typed properties
Console.WriteLine("Recursive Directory Files:");
Console.WriteLine($"  SomeFile.txt: {ProjectFiles.ProjectFiles.RecursiveDirectory.SomeFileTxt}");
Console.WriteLine($"  Exists: {File.Exists(ProjectFiles.ProjectFiles.RecursiveDirectory.SomeFileTxt)}");
Console.WriteLine();

Console.WriteLine($"  Nested in SubDir: {ProjectFiles.ProjectFiles.RecursiveDirectory.SubDir.NestedFileTxt}");
Console.WriteLine($"  Exists: {File.Exists(ProjectFiles.ProjectFiles.RecursiveDirectory.SubDir.NestedFileTxt)}");
Console.WriteLine();

Console.WriteLine("Specific Directory Files:");
Console.WriteLine($"  Dir1/File1.txt: {ProjectFiles.ProjectFiles.SpecificDirectory.Dir1.File1Txt}");
Console.WriteLine($"  Dir1/File2.txt: {ProjectFiles.ProjectFiles.SpecificDirectory.Dir1.File2Txt}");
Console.WriteLine($"  Dir2/File4.txt: {ProjectFiles.ProjectFiles.SpecificDirectory.Dir2.File4Txt}");
Console.WriteLine($"  File3.txt: {ProjectFiles.ProjectFiles.SpecificDirectory.File3Txt}");
Console.WriteLine();

Console.WriteLine("Config Files:");
Console.WriteLine($"  appsettings.json: {ProjectFiles.ProjectFiles.Config.AppsettingsJson}");
Console.WriteLine($"  Exists: {File.Exists(ProjectFiles.ProjectFiles.Config.AppsettingsJson)}");
Console.WriteLine();

// Read content from files
Console.WriteLine("Reading file contents:");
if (File.Exists(ProjectFiles.ProjectFiles.SpecificDirectory.Dir1.File1Txt))
{
    var content = File.ReadAllText(ProjectFiles.ProjectFiles.SpecificDirectory.Dir1.File1Txt);
    Console.WriteLine($"  File1.txt content: {content.Trim()}");
}

if (File.Exists(ProjectFiles.ProjectFiles.Config.AppsettingsJson))
{
    var jsonContent = File.ReadAllText(ProjectFiles.ProjectFiles.Config.AppsettingsJson);
    Console.WriteLine($"  appsettings.json: {jsonContent.Trim()}");
}

Console.WriteLine();
Console.WriteLine("=== Demo Complete ===");
