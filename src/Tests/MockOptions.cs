class MockOptions(Dictionary<string, string> options) :
    AnalyzerConfigOptions
{
    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) =>
        options.TryGetValue(key, out value);
}