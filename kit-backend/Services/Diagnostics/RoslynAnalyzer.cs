using Microsoft.CodeAnalysis.CSharp;

public class RoslynAnalyzer
{
    public static List<string> Analyze(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var diagnostics = syntaxTree.GetDiagnostics();

        return diagnostics.Select(d => d.ToString()).ToList();
    }
}
