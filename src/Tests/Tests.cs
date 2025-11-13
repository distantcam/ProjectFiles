[TestFixture]
public class Tests
{
    [Test]
    public Task SimpleFile()
    {
        var project = """
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

        return TestHelper.Verify(project, ["config.json"]);
    }

    [Test]
    public Task NestedDirectories()
    {
        var project = """
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

        return TestHelper.Verify(
            project,
            [
                "Assets/Images/logo.png",
                "Assets/Data/users.csv"
            ]);
    }

    [Test]
    public Task RecursiveGlob()
    {
        var project = """
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

        return TestHelper.Verify(
            project,
            [
                "Assets/file1.txt",
                "Assets/SubDir/file2.json",
                "Assets/SubDir/Nested/file3.xml"
            ]);
    }

    [Test]
    public Task MultipleFileTypes()
    {
        var project = """
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

        return TestHelper.Verify(
            project,
            [
                "Data/users.csv",
                "wwwroot/index.html",
                "Config/appsettings.json"
            ]);
    }

    [Test]
    public Task SpecialCharacters()
    {
        var project = """
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

        return TestHelper.Verify(
            project,
            [
                "my-config.json",
                "user_data.csv",
                "file.with.dots.txt"
            ]);
    }

    [Test]
    public Task Numbers()
    {
        var project = """
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

        return TestHelper.Verify(
            project,
            [
                "1-first.txt",
                "file123.json"
            ]);
    }

    [Test]
    public Task EmptyDirectories()
    {
        var project = """
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
        return TestHelper.Verify(project, []);
    }

    [Test]
    public Task WildcardPatterns()
    {
        var project = """
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

        return TestHelper.Verify(
            project,
            [
                "Configs/appsettings.json",
                "Configs/logging.json",
                "Configs/database.json"
            ]);
    }

    [Test]
    public Task KeywordsAsFileNames()
    {
        var project = """
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

        return TestHelper.Verify(
            project,
            [
                "class.txt",
                "namespace.json",
                "static.xml"
            ]);
    }

    [Test]
    public Task ComplexHierarchy()
    {
        var project = """
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

        return TestHelper.Verify(
            project,
            [
                "Assets/Images/Icons/favicon.ico",
                "Assets/Images/Icons/logo.png",
                "Assets/Images/Backgrounds/header.jpg",
                "Assets/Data/Config/settings.json",
                "Assets/Data/Config/secrets.json",
                "Assets/Data/Seeds/users.csv",
                "Assets/Fonts/roboto.ttf",
                "Assets/readme.txt"
            ]);
    }

    [Test]
    public Task NoFilesMarked()
    {
        var project = """
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

        return TestHelper.Verify(project, []);
    }

    [Test]
    public Task IgnoresFilesWithCopyToOutputDirectoryNever()
    {
        var project = """
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

        return TestHelper.Verify(project, ["included.txt", "excluded.txt"]);
    }
}