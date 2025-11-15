namespace ProjectFilesGenerator;

abstract class ProjectDirectory(string path)
{
    public string Path { get; } = path;
    public override string ToString() => Path;
    public static implicit operator string(ProjectDirectory temp) =>
        temp.Path;

    public static implicit operator FileInfo(ProjectDirectory temp) =>
        new(temp.Path);

    public DirectoryInfo Info => new(Path);
}