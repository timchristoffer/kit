using System.Text.RegularExpressions;

namespace KitBackend.Services.AnalysisHelpers
{
    public static class ComplexityHelper
    {
        private static readonly Dictionary<string, int> ComplexityKeywordPenalties = new Dictionary<string, int>
        {
            { "interface", 20 }, { "abstract", 20 }, { "virtual", 20 }, { "override", 20 }, { "sealed", 20 }, { "static", 20 },
            { "readonly", 20 }, { "const", 20 }, { "enum", 20 }, { "struct", 20 }, { "delegate", 20 }, { "event", 20 },
            { "operator", 20 }, { "implicit", 40 }, { "explicit", 40 }, { "params", 20 }, { "ref", 20 }, { "out", 20 }, { "in", 20 },
            { "yield", 40 }, { "async", 20 }, { "await", 20 }, { "lock", 20 }, { "using", 20 }, { "fixed", 20 }, { "unsafe", 20 },
            { "checked", 20 }, { "unchecked", 40 }, { "sizeof", 20 }, { "typeof", 20 }, { "nameof", 20 }, { "default", 20 },
            { "switch", 20 }, { "case", 20 }, { "goto", 20 }, { "continue", 20 }, { "break", 20 }, { "try", 20 }, { "catch", 20 },
            { "finally", 20 }, { "throw", 20 }, { "return", 20 }
        };

        public static int CalculateComplexityScore(string code)
        {
            if (string.IsNullOrEmpty(code))
                return 100;

            int score = 100;

            // Cyclomatic Complexity
            int cyclomaticComplexity = CalculateCyclomaticComplexity(code);
            if (cyclomaticComplexity > 10)
                score -= 20;

            // Class Member Count
            if (code.Split(new string[] { "public", "private", "protected" }, StringSplitOptions.None).Length > 20)
                score -= 10;

            // Inheritance
            if (Regex.IsMatch(code, @"\bclass\b\s+\w+\s*:\s*\w+"))
                score -= 10;

            // Create a set of keywords found in the code
            HashSet<string> foundKeywords = new HashSet<string>();
            foreach (var keyword in ComplexityKeywordPenalties.Keys)
            {
                if (Regex.IsMatch(code, $@"\b{keyword}\b"))
                {
                    foundKeywords.Add(keyword);
                }
            }

            // Apply penalties for each unique keyword just once
            foreach (var keyword in foundKeywords)
            {
                score -= ComplexityKeywordPenalties[keyword];
            }

            return Math.Max(0, score);
        }

        private static int CalculateCyclomaticComplexity(string code)
        {
            int complexity = 1;
            complexity += code.Split(new string[] {
                "if", "else", "case", "for", "while", "&&", "||", "switch",
                "catch", "?", "do", "goto", "continue", "break", "function",
                "=>", "try", "finally"
            }, StringSplitOptions.None).Length - 1;
            return complexity;
        }
    }
}
