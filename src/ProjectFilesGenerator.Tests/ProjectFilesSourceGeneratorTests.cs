using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ProjectFilesGenerator;
using VerifyXunit;
using Xunit;

namespace ProjectFilesGenerator.Tests;

[UsesVerify]
public class ProjectFilesSourceGeneratorTests
{
    [Fact]
    public Task GeneratesCorrectCodeForSimpleFile()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="config.json">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent, "config.json");
    }

    [Fact]
    public Task GeneratesCorrectCodeForNestedDirectories()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="Assets\Images\logo.png">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
                <None Update="Assets\Data\users.csv">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent, 
            "Assets/Images/logo.png",
            "Assets/Data/users.csv");
    }

    [Fact]
    public Task GeneratesCorrectCodeForRecursiveGlob()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="Assets\**\*.*">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent,
            "Assets/file1.txt",
            "Assets/SubDir/file2.json",
            "Assets/SubDir/Nested/file3.xml");
    }

    [Fact]
    public Task GeneratesCorrectCodeForMultipleFileTypes()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="Data\users.csv">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
                <Content Include="wwwroot\index.html">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </Content>
                <None Update="Config\appsettings.json">
                  <CopyToOutputDirectory>Always</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent,
            "Data/users.csv",
            "wwwroot/index.html",
            "Config/appsettings.json");
    }

    [Fact]
    public Task HandlesFilesWithSpecialCharacters()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="my-config.json">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
                <None Update="user_data.csv">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
                <None Update="file.with.dots.txt">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent,
            "my-config.json",
            "user_data.csv",
            "file.with.dots.txt");
    }

    [Fact]
    public Task HandlesFilesWithNumbers()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="1-first.txt">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
                <None Update="file123.json">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent,
            "1-first.txt",
            "file123.json");
    }

    [Fact]
    public Task HandlesEmptyDirectories()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="EmptyDir\**\*.*">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        // No files in EmptyDir - should generate empty or minimal code
        return TestHelper.Verify(projectContent);
    }

    [Fact]
    public Task GeneratesCodeForWildcardPatterns()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="Configs\*.json">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent,
            "Configs/appsettings.json",
            "Configs/logging.json",
            "Configs/database.json");
    }

    [Fact]
    public Task HandlesCSharpKeywordsAsFileNames()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="class.txt">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
                <None Update="namespace.json">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
                <None Update="static.xml">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent,
            "class.txt",
            "namespace.json",
            "static.xml");
    }

    [Fact]
    public Task GeneratesCodeForComplexHierarchy()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="Assets\**\*.*">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent,
            "Assets/Images/Icons/favicon.ico",
            "Assets/Images/Icons/logo.png",
            "Assets/Images/Backgrounds/header.jpg",
            "Assets/Data/Config/settings.json",
            "Assets/Data/Config/secrets.json",
            "Assets/Data/Seeds/users.csv",
            "Assets/Fonts/roboto.ttf",
            "Assets/readme.txt");
    }

    [Fact]
    public Task GeneratesNothingWhenNoFilesMarked()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="somefile.txt">
                  <!-- No CopyToOutputDirectory -->
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent);
    }

    [Fact]
    public Task IgnoresFilesWithCopyToOutputDirectoryNever()
    {
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <None Update="included.txt">
                  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                </None>
                <None Update="excluded.txt">
                  <CopyToOutputDirectory>Never</CopyToOutputDirectory>
                </None>
              </ItemGroup>
            </Project>
            """;

        return TestHelper.Verify(projectContent, "included.txt", "excluded.txt");
    }
}
