namespace KitBackend.Services.AnalysisHelpers
{
    public static class ComplexityHelper
    {
        public static int CalculateComplexityScore(string code)
        {
            int score = 100;
            int cyclomaticComplexity = CalculateCyclomaticComplexity(code);
            if (cyclomaticComplexity > 10) score -= 20;
            if (code.Split(new string[] { "public", "private", "protected" }, StringSplitOptions.None).Length > 20) score -= 10;
            if (code.Contains("class") && code.Contains(":")) score -= 10;
            if (code.Contains("interface")) score -= 10;
            if (code.Contains("abstract")) score -= 10;
            if (code.Contains("virtual")) score -= 10;
            if (code.Contains("override")) score -= 10;
            if (code.Contains("sealed")) score -= 10;
            if (code.Contains("static")) score -= 10;
            if (code.Contains("readonly")) score -= 10;
            if (code.Contains("const")) score -= 10;
            if (code.Contains("enum")) score -= 10;
            if (code.Contains("struct")) score -= 10;
            if (code.Contains("delegate")) score -= 10;
            if (code.Contains("event")) score -= 10;
            if (code.Contains("operator")) score -= 10;
            if (code.Contains("implicit")) score -= 10;
            if (code.Contains("explicit")) score -= 10;
            if (code.Contains("params")) score -= 10;
            if (code.Contains("ref")) score -= 10;
            if (code.Contains("out")) score -= 10;
            if (code.Contains("in")) score -= 10;
            if (code.Contains("yield")) score -= 10;
            if (code.Contains("async")) score -= 10;
            if (code.Contains("await")) score -= 10;
            if (code.Contains("lock")) score -= 10;
            if (code.Contains("using")) score -= 10;
            if (code.Contains("fixed")) score -= 10;
            if (code.Contains("unsafe")) score -= 10;
            if (code.Contains("checked")) score -= 10;
            if (code.Contains("unchecked")) score -= 10;
            if (code.Contains("sizeof")) score -= 10;
            if (code.Contains("typeof")) score -= 10;
            if (code.Contains("nameof")) score -= 10;
            if (code.Contains("default")) score -= 10;
            if (code.Contains("switch")) score -= 10;
            if (code.Contains("case")) score -= 10;
            if (code.Contains("goto")) score -= 10;
            if (code.Contains("continue")) score -= 10;
            if (code.Contains("break")) score -= 10;
            if (code.Contains("try")) score -= 10;
            if (code.Contains("catch")) score -= 10;
            if (code.Contains("finally")) score -= 10;
            if (code.Contains("throw")) score -= 10;
            if (code.Contains("return")) score -= 10;
            if (code.Contains("yield")) score -= 10;
            if (code.Contains("await")) score -= 10;
            if (code.Contains("async")) score -= 10;
            if (code.Contains("lock")) score -= 10;
            if (code.Contains("using")) score -= 10;
            if (code.Contains("fixed")) score -= 10;
            if (code.Contains("unsafe")) score -= 10;
            if (code.Contains("checked")) score -= 10;
            if (code.Contains("unchecked")) score -= 10;
            if (code.Contains("sizeof")) score -= 10;
            if (code.Contains("typeof")) score -= 10;
            if (code.Contains("nameof")) score -= 10;
            if (code.Contains("default")) score -= 10;

            return Math.Max(0, score);
        }

        private static int CalculateCyclomaticComplexity(string code)
        {
            int complexity = 1;
            complexity += code.Split(new string[] { "if", "else", "case", "for", "while", "&&", "||", "switch", "catch", "?", "do", "goto", "continue", "break", "function", "=>", "try", "finally" }, StringSplitOptions.None).Length - 1;
            return complexity;
        }
    }
}
