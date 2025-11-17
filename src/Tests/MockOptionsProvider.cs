class MockOptionsProvider(Dictionary<string, Dictionary<string, string>> fileMetadata) :
    AnalyzerConfigOptionsProvider
{
    public override AnalyzerConfigOptions GlobalOptions { get; } = new MockOptions(new());

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) =>
        new MockOptions([]);

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        var options = fileMetadata.TryGetValue(textFile.Path, out var metadata)
            ? metadata
            : [];

        return new MockOptions(options);
    }
}