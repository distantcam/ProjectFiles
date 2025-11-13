# ProjectFilesGenerator.Tests

Comprehensive test suite for the ProjectFiles source generator using [Verify.SourceGenerators](https://github.com/VerifyTests/Verify.SourceGenerators).

## What is Verify.SourceGenerators?

Verify.SourceGenerators is a snapshot testing library specifically designed for C# source generators. Instead of writing complex assertions, you verify that the generator produces the expected output by comparing against committed snapshots.

## Test Structure

### ProjectFilesSourceGeneratorTests.cs

Contains all test cases for different scenarios:

- ✅ Simple single files
- ✅ Nested directory structures
- ✅ Recursive glob patterns (`**\*.*`)
- ✅ Multiple file types (None, Content, etc.)
- ✅ Special characters in filenames
- ✅ Files starting with numbers
- ✅ Empty directories
- ✅ Wildcard patterns (`*.json`)
- ✅ C# keywords as filenames
- ✅ Complex hierarchies
- ✅ Edge cases (no files, excluded files)

### TestHelper.cs

Provides utility methods for:
- Setting up temporary test directories
- Creating mock project files
- Running the generator
- Verifying output with snapshots

## Running Tests

### In JetBrains Rider

1. Open the solution
2. Right-click on `ProjectFilesGenerator.Tests` project
3. Click "Run Unit Tests"
4. Or use `Ctrl+U, R` to run all tests

### Using .NET CLI

```bash
# Run all tests
dotnet test

# Run a specific test
dotnet test --filter "FullyQualifiedName~GeneratesCorrectCodeForSimpleFile"

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### In Visual Studio

1. Open Test Explorer (Test → Test Explorer)
2. Click "Run All Tests"
3. Or right-click individual tests to run them

## How Snapshot Testing Works

### First Run (Creating Snapshots)

When you first run a test:

1. The generator runs with the test input
2. The output is captured
3. A `.verified.txt` file is created in the `Snapshots/` directory
4. You review the output to ensure it's correct
5. Commit the `.verified.txt` file to source control

### Subsequent Runs (Verifying Snapshots)

On future test runs:

1. The generator runs with the test input
2. The output is captured
3. The output is compared against the `.verified.txt` file
4. If they match: ✅ Test passes
5. If they differ: ❌ Test fails and shows a diff

### When Output Changes

If the generator's output changes (intentionally):

1. Run the tests - they will fail
2. Review the diff using Rider's diff tool
3. If the new output is correct, accept it:
   - Rider: Click "Accept" in the diff viewer
   - CLI: Delete the old `.verified.txt` and rename `.received.txt` to `.verified.txt`
4. Commit the updated `.verified.txt` file

## Snapshot Location

Snapshots are stored in:
```
ProjectFilesGenerator.Tests/
└── Snapshots/
    └── ProjectFilesSourceGeneratorTests/
        ├── GeneratesCorrectCodeForSimpleFile.verified.txt
        ├── GeneratesCorrectCodeForNestedDirectories.verified.txt
        ├── GeneratesCorrectCodeForRecursiveGlob.verified.txt
        └── ... (one file per test)
```

## Example Test

```csharp
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
```

This test:
1. Creates a mock `.csproj` file
2. Creates a mock `config.json` file
3. Runs the generator
4. Verifies the output matches the snapshot

## Adding New Tests

1. Add a new `[Fact]` method to `ProjectFilesSourceGeneratorTests.cs`
2. Create your test scenario with `TestHelper.Verify()`
3. Run the test
4. Review the generated snapshot in `Snapshots/`
5. If correct, commit the snapshot file
6. Done! Future runs will verify against this snapshot

## Benefits of Snapshot Testing

### ✅ Advantages

- **Easy to write**: No complex assertions needed
- **Comprehensive**: Verifies entire generated output
- **Visual**: Easy to see what changed in diffs
- **Maintainable**: Just accept new snapshots when intentional changes occur
- **Documentation**: Snapshots serve as examples of expected output

### ⚠️ Considerations

- Must review snapshots carefully before accepting
- Snapshots must be committed to source control
- Can generate many files if you have many tests
- Need to keep snapshots up to date

## Debugging Failed Tests

### In Rider

1. When a test fails, Rider shows a diff viewer
2. Left side: Expected (from `.verified.txt`)
3. Right side: Actual (from `.received.txt`)
4. Review the differences
5. If correct: Click "Accept" to update the snapshot
6. If incorrect: Fix the generator and rerun

### Using Git Diff

```bash
# View what changed in snapshots
git diff Snapshots/

# Compare specific snapshot
diff Snapshots/.../TestName.verified.txt Snapshots/.../TestName.received.txt
```

## Continuous Integration

Tests work great in CI/CD:

```yaml
# Example GitHub Actions
- name: Run Tests
  run: dotnet test --no-build --verbosity normal

# Tests will fail if generator output doesn't match committed snapshots
```

## Best Practices

1. **Review snapshots carefully** before accepting them
2. **Commit snapshots** to version control
3. **Use descriptive test names** that explain the scenario
4. **Test edge cases** (empty input, special characters, etc.)
5. **Keep tests focused** - one scenario per test
6. **Update snapshots intentionally** when generator logic changes

## Troubleshooting

### Test creates .received.txt but not .verified.txt

This is normal on first run. Review the `.received.txt` file and if it looks correct, rename it to `.verified.txt`.

### Tests fail after updating generator

This is expected! Review the diffs and accept the new snapshots if the changes are intentional.

### Can't find snapshot files

Check `Snapshots/ProjectFilesSourceGeneratorTests/` directory. Verify that:
- The test ran successfully
- The `VerifierSettings` are configured correctly
- You have write permissions to the directory

### Snapshots not updating in Rider

Try:
- Clean and rebuild the solution
- Manually delete `.received.txt` files and rerun
- Check that Verify.SourceGenerators is properly installed

## Further Reading

- [Verify Documentation](https://github.com/VerifyTests/Verify)
- [Verify.SourceGenerators Documentation](https://github.com/VerifyTests/Verify.SourceGenerators)
- [Source Generators Testing Guide](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md#unit-testing-of-generators)

## Quick Reference

```bash
# Run all tests
dotnet test

# Run tests and accept all new snapshots (careful!)
# (There's no automatic way - must review each one)

# Clean up test artifacts
dotnet clean

# View test coverage (requires coverlet)
dotnet test /p:CollectCoverage=true
```

Happy Testing! 🧪
