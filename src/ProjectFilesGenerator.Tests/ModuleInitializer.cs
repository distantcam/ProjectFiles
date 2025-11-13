using System.Runtime.CompilerServices;
using VerifyTests;

namespace ProjectFilesGenerator.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // Initialize Verify for source generators
        VerifySourceGenerators.Initialize();
        
        // Configure Verify to use better diffs
        VerifierSettings.UseStrictJson();
        
        // Only verify generated source, not diagnostics
        VerifierSettings.ScrubEmptyLines();
    }
}
