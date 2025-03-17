namespace KitBackend.Services.AnalysisHelpers
{
    public static class ReadabilityHelper
    {
        public static int CalculateReadabilityScore(string code)
        {
            int score = 100;
            if (code.Split('\n').Length > 500) score -= 20;
            if (code.Contains("switch") && code.Contains("case")) score -= 10;
            if (code.Length > 1000) score -= 15;
            if (code.Contains("todo")) score -= 5;
            if (code.Contains("fixme")) score -= 5;
            if (code.Split('\n').Any(line => line.Length > 120)) score -= 10;
            if (code.Count(c => c == '{') > 20) score -= 10;
            if (!code.Contains("//")) score -= 10;
            if (code.Contains("var ")) score -= 5;
            if (code.Contains("magic number")) score -= 5;
            if (!code.Contains("PascalCase") || !code.Contains("camelCase")) score -= 5;
            if (code.Contains("if") && !code.Contains("else")) score -= 5;
            if (code.Split(new string[] { "public", "private", "protected" }, StringSplitOptions.None).Any(func => func.Length > 1000)) score -= 10; // Check for long functions
            if (code.Contains("goto")) score -= 10;
            if (code.Contains("continue")) score -= 5;
            if (code.Contains("break")) score -= 5;
            if (code.Contains("try") && !code.Contains("catch")) score -= 10;
            if (code.Contains("catch") && !code.Contains("finally")) score -= 5;
            if (code.Contains("while") && !code.Contains("do")) score -= 5;
            if (code.Contains("do") && !code.Contains("while")) score -= 5;

            return Math.Max(0, score);
        }

        public static List<string> AnalyzeReadability(string code)
        {
            var issues = new List<string>();

            if (code.Split('\n').Length > 500)
            {
                issues.Add("Code readability: File exceeds 500 lines. Consider breaking it into smaller modules.");
            }

            if (code.Contains("if") && !code.Contains("else"))
            {
                issues.Add("Code style: Unmatched if statements detected. Consider adding else or comments.");
            }

            if (code.Contains("switch") && code.Contains("case"))
            {
                issues.Add("Code style: Complex switch statements detected. Consider refactoring to improve readability.");
            }

            var functions = code.Split(new string[] { "public", "private", "protected" }, StringSplitOptions.None);
            foreach (var func in functions)
            {
                if (func.Length > 1000)
                {
                    issues.Add("Code readability: Function exceeds 1000 characters. Consider splitting into smaller functions.");
                }
            }

            if (code.Contains("todo"))
            {
                issues.Add("Code readability: TODO comments found, ensure they are addressed before finalizing.");
            }

            if (code.Split('\n').Any(line => line.Length > 120))
            {
                issues.Add("Code readability: Long lines detected. Consider breaking them into shorter lines.");
            }

            if (code.Count(c => c == '{') > 20)
            {
                issues.Add("Code readability: Deeply nested structures detected. Consider refactoring to improve readability.");
            }

            if (!code.Contains("//"))
            {
                issues.Add("Code readability: Lack of comments detected. Consider adding comments to improve readability.");
            }

            if (code.Contains("var "))
            {
                issues.Add("Code readability: Usage of var detected. Consider using explicit types for better readability.");
            }

            if (code.Contains("magic number"))
            {
                issues.Add("Code readability: Magic numbers detected. Consider defining constants for better readability.");
            }

            if (!code.Contains("PascalCase") || !code.Contains("camelCase"))
            {
                issues.Add("Code readability: Inconsistent naming conventions detected. Ensure consistent use of PascalCase and camelCase.");
            }

            return issues;
        }
    }
}
