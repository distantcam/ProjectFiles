namespace ProjectFilesGenerator;

class ProjectFile(string path)
{
    public string Path { get; } = path;
    public override string ToString() => Path;

    public static implicit operator string(ProjectFile temp) =>
        temp.Path;

    public static implicit operator FileInfo(ProjectFile temp) =>
        new(temp.Path);

    public FileInfo Info => new(Path);
}