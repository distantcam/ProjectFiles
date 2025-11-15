//HintName: ProjectFiles.ProjectFile.g.cs
namespace ProjectFilesGenerator;

partial class ProjectFile(string path)
{
    public string Path { get; } = path;

    public override string ToString() => Path;

    public static implicit operator string(ProjectFile temp) =>
        temp.Path;

    public static implicit operator FileInfo(ProjectFile temp) =>
        new(temp.Path);

    public FileStream OpenRead() =>
        File.OpenRead(Path);

    public StreamReader OpenText() =>
        File.OpenText(Path);

    public string ReadAllText() =>
        File.ReadAllText(Path);

    public Task<string> ReadAllTextAsync() =>
        File.ReadAllTextAsync(Path);

    public FileInfo Info => new(Path);
}