class MockAdditionalText(string path, string text) :
    AdditionalText
{
    public override string Path { get; } = path;

    public override SourceText GetText(Cancel cancel = default) =>
        SourceText.From(text, Encoding.UTF8);
}