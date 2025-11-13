# 📋 File Index

## 📖 Documentation (Start Here!)

| File | Description | Read Time |
|------|-------------|-----------|
| **[PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md)** | Complete project overview and navigation guide | 5 min |
| **[QUICKSTART.md](QUICKSTART.md)** | Get started in 5 minutes | 3 min |
| **[README.md](README.md)** | Full documentation with all details | 15 min |

## 🔧 Source Code

### Source Generator Project

| File | Description |
|------|-------------|
| `ProjectFilesGenerator/ProjectFilesGenerator.csproj` | Generator project file (netstandard2.0) |
| `ProjectFilesGenerator/ProjectFilesSourceGenerator.cs` | Main generator implementation (370 lines) |

**Key Classes:**
- `ProjectFilesSourceGenerator` - Main generator class implementing `IIncrementalGenerator`
- `ParseProjectFile()` - Parses .csproj XML to find files with CopyToOutputDirectory
- `ExpandGlobPattern()` - Resolves glob patterns like `**\*.*`
- `BuildFileTree()` - Creates hierarchical directory/file structure
- `GenerateSource()` - Produces the final C# code
- `ToValidIdentifier()` - Converts filenames to valid C# identifiers

### Example Application

| File | Description |
|------|-------------|
| `ExampleApp/ExampleApp.csproj` | Example project showing how to use the generator |
| `ExampleApp/Program.cs` | Usage examples demonstrating the generated API |

### Example Files (For Testing)

```
ExampleApp/
├── Config/
│   └── appsettings.json
├── RecursiveDirectory/
│   ├── SomeFile.txt
│   └── SubDir/
│       └── NestedFile.txt
└── SpecificDirectory/
    ├── File3.txt
    ├── Dir1/
    │   ├── File1.txt
    │   └── File2.txt
    └── Dir2/
        └── File4.txt
```

## 📄 Sample Output

| File | Description |
|------|-------------|
| `ExampleGeneratedOutput.cs` | Example of what the generator produces |

This shows the nested class structure and properties generated for the example files.

## 🏗️ Solution File

| File | Description |
|------|-------------|
| `ProjectFilesGenerator.sln` | Visual Studio solution file |

## 🚀 Quick Start Checklist

- [ ] Read [PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md) for project structure
- [ ] Read [QUICKSTART.md](QUICKSTART.md) for setup instructions
- [ ] Open `ProjectFilesGenerator.sln` in Rider
- [ ] Build the solution
- [ ] Run `ExampleApp` to see it in action
- [ ] View generated code at: `ExampleApp/obj/Debug/net10.0/generated/ProjectFilesGenerator/ProjectFilesGenerator.ProjectFilesSourceGenerator/ProjectFiles.g.cs`
- [ ] Check [ExampleGeneratedOutput.cs](ExampleGeneratedOutput.cs) to see expected output
- [ ] Integrate into your own project following [QUICKSTART.md](QUICKSTART.md)

## 📊 File Statistics

| Category | Count | Total Lines |
|----------|-------|-------------|
| Documentation | 3 | ~450 lines |
| Source Code | 2 | ~400 lines |
| Example Code | 2 | ~80 lines |
| Example Files | 8 | ~30 lines |
| **Total** | **15** | **~960 lines** |

## 🔗 Recommended Reading Order

### For Quick Start (10 minutes)
1. [PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md) - Understand the project
2. [QUICKSTART.md](QUICKSTART.md) - Get it working
3. Open and run `ExampleApp/Program.cs`

### For Deep Understanding (30 minutes)
1. [PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md)
2. [README.md](README.md) - Full details
3. `ProjectFilesSourceGenerator.cs` - Read the implementation
4. [ExampleGeneratedOutput.cs](ExampleGeneratedOutput.cs) - See the output
5. `ExampleApp/ExampleApp.csproj` - Integration example

### For Implementation (5 minutes)
1. [QUICKSTART.md](QUICKSTART.md) - Follow the steps
2. Copy the two `<ItemGroup>` blocks to your `.csproj`
3. Add your files with `CopyToOutputDirectory`
4. Build and use!

## 💻 Command Reference

```bash
# Build the solution
dotnet build ProjectFilesGenerator.sln

# Run the example app
dotnet run --project ExampleApp/ExampleApp.csproj

# Clean solution
dotnet clean ProjectFilesGenerator.sln

# View generated code (Windows)
type ExampleApp\obj\Debug\net10.0\generated\ProjectFilesGenerator\ProjectFilesGenerator.ProjectFilesSourceGenerator\ProjectFiles.g.cs
```

## 🎯 Key Concepts

| Concept | File Reference | Description |
|---------|---------------|-------------|
| **Source Generator** | `ProjectFilesSourceGenerator.cs` | Runs at compile time to generate code |
| **Incremental Generation** | Line 9-13 in generator | Only regenerates when inputs change |
| **Glob Expansion** | `ExpandGlobPattern()` method | Resolves `**\*.*` patterns to actual files |
| **Tree Structure** | `BuildFileTree()` method | Creates nested class hierarchy |
| **Identifier Sanitization** | `ToValidIdentifier()` method | Converts filenames to valid C# |
| **MSBuild Integration** | `.csproj` files | How projects reference the generator |

## 📚 Learn More About

### Source Generators
- [Microsoft Docs: Source Generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
- [Incremental Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)

### MSBuild
- [AdditionalFiles](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#additionalfiles)
- [CopyToOutputDirectory](https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-items#none)

## 🎉 You're Ready!

All the files you need are here. Start with [PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md) and you'll be up and running in minutes!
