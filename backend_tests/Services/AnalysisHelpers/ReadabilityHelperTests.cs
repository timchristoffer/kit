using KitBackend.Services.AnalysisHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace KitBackend.Tests.Services.AnalysisHelpers
{
    public class ReadabilityHelperTests
    {
        [Theory]
        [InlineData("public void Test() { }", 85)] // Minimal code, men saknar camelCase/PascalCase och kommentarer
        [InlineData("public void Test() { \n if (x > 0) { return; } \n }", 80)] // Simple if without else
        [InlineData("public void Test() { \n var x = 10; \n }", 80)] // Using 'var'
        [InlineData("public void Test() { \n switch(x) { case 1: break; } \n }", 70)] // Switch with case
        [InlineData("public void Test() { \n try { } \n }", 75)] // Try without catch
        [InlineData("public void Test() { \n try { } catch { } \n }", 80)] // Try with catch, no finally
        [InlineData("public void Test() { \n while(true) { } \n }", 80)] // While without do
        [InlineData("public void Test() { \n do { } \n }", 80)] // Do without while
        [InlineData("public void Test() { \n goto Label; \n Label: return; \n }", 75)] // Using goto
        [InlineData("public void Test() { \n for(;;) { continue; } \n }", 80)] // Using continue
        [InlineData("public void Test() { \n for(;;) { break; } \n }", 80)] // Using break
        [InlineData("// A comment\npublic void Test() { }", 95)] // Has comments, but still missing naming conventions
        [InlineData("// A comment\npublic void Test() { int camelCase = 0; String PascalCase = null; }", 100)] // Complete good code
        [InlineData("public void Test() { \n // TODO: Fix this \n }", 90)] // Contains TODO
        public void CalculateReadabilityScore_ShouldCalculateCorrectScore(string code, int expectedScore)
        {
            // Act
            int score = ReadabilityHelper.CalculateReadabilityScore(code);

            // Assert
            Assert.Equal(expectedScore, score);
        }

        [Theory]
        [InlineData(501, 50)] // More than 500 lines
        [InlineData(2000, 50)] // More than 500 lines and over 1000 characters
        public void CalculateReadabilityScore_ShouldDecreaseForLongCode(int lineCount, int expectedScore)
        {
            // Arrange
            string code = string.Join("\n", Enumerable.Repeat("var x = 0;", lineCount));
            code = "// Comment\n" + code + "\nint camelCase = 0; String PascalCase = null;"; // Add necessary parts for baseline score

            // Act
            int score = ReadabilityHelper.CalculateReadabilityScore(code);

            // Assert
            Assert.Equal(expectedScore, score);
        }

        [Fact]
        public void CalculateReadabilityScore_ShouldDecreaseForLongLines()
        {
            // Arrange
            string longLine = new string('x', 121); // Line longer than 120 characters
            string code = $"// Comment\npublic void Test() {{ \n {longLine} \n }}\nint camelCase = 0; String PascalCase = null;";

            // Act
            int score = ReadabilityHelper.CalculateReadabilityScore(code);

            // Assert
            Assert.Equal(90, score); // 100 - 10 for long line
        }

        [Fact]
        public void CalculateReadabilityScore_ShouldDecreaseForDeeplyNested()
        {
            // Arrange
            string nestedCode = string.Join("", Enumerable.Repeat("{", 21)) +
                                string.Join("", Enumerable.Repeat("}", 21));
            string code = $"// Comment\npublic void Test() {{ {nestedCode} }}\nint camelCase = 0; String PascalCase = null;";

            // Act
            int score = ReadabilityHelper.CalculateReadabilityScore(code);

            // Assert
            Assert.Equal(90, score); // 100 - 10 for deep nesting
        }

        [Fact]
        public void CalculateReadabilityScore_ShouldDecreaseForInconsistentNaming()
        {
            // Arrange
            string code = "// Comment\npublic void test() { int X = 10; }"; // No camelCase or PascalCase correctly used

            // Act
            int score = ReadabilityHelper.CalculateReadabilityScore(code);

            // Assert
            Assert.Equal(95, score); // 100 - 10 for no comments - 5 for inconsistent naming
        }

        [Theory]
        [InlineData("public void Test() { \n if (x > 0) { return; } \n }", "Code style: Unmatched if statements detected.")]
        [InlineData("public void Test() { \n switch(x) { case 1: break; } \n }", "Code style: Complex switch statements detected.")]
        [InlineData("public void Test() { \n var x = 10; \n }", "Code readability: Usage of var detected.")]
        public void AnalyzeReadability_ShouldIdentifyIssues(string code, string expectedIssue)
        {
            // Act
            List<string> issues = ReadabilityHelper.AnalyzeReadability(code);

            // Assert
            Assert.Contains(issues, issue => issue.StartsWith(expectedIssue));
        }

        [Fact]
        public void AnalyzeReadability_ShouldIdentifyMultipleIssues()
        {
            // Arrange
            string code = @"
            public void Test() { 
                var x = 10; 
                if (x > 0) { return; }
                switch(x) { 
                    case 1: break; 
                    case 2: break;
                    case 3: break;
                } 
            }";

            // Act
            List<string> issues = ReadabilityHelper.AnalyzeReadability(code);

            // Assert
            // Expect 5 issues: var, if without else, switch, no comments, inconsistent naming
            Assert.Equal(5, issues.Count);
            Assert.Contains(issues, i => i.Contains("Unmatched if statements"));
            Assert.Contains(issues, i => i.Contains("Complex switch statements"));
            Assert.Contains(issues, i => i.Contains("Usage of var detected"));
            Assert.Contains(issues, i => i.Contains("Lack of comments"));
            Assert.Contains(issues, i => i.Contains("Inconsistent naming conventions"));
        }

        [Fact]
        public void AnalyzeReadability_ShouldIdentifyLongFunctions()
        {
            // Arrange
            string longFunction = new string('x', 1001); // Function body longer than 1000 chars
            string code = $"public void Test() {{ {longFunction} }}";

            // Act
            List<string> issues = ReadabilityHelper.AnalyzeReadability(code);

            // Assert
            Assert.Contains(issues, issue => issue.Contains("Function exceeds 1000 characters"));
        }

        [Fact]
        public void AnalyzeReadability_ShouldIdentifyLackOfComments()
        {
            // Arrange
            string code = "public void Test() { int x = 10; return; }"; // No comments

            // Act
            List<string> issues = ReadabilityHelper.AnalyzeReadability(code);

            // Assert
            Assert.Contains(issues, issue => issue.Contains("Lack of comments detected"));
        }

        [Fact]
        public void AnalyzeReadability_ShouldIdentifyLongLines()
        {
            // Arrange
            string longLine = new string('x', 121); // Line longer than 120 characters
            string code = $"public void Test() {{ \n {longLine} \n }}";

            // Act
            List<string> issues = ReadabilityHelper.AnalyzeReadability(code);

            // Assert
            Assert.Contains(issues, issue => issue.Contains("Long lines detected"));
        }

        [Fact]
        public void AnalyzeReadability_ShouldIdentifyDeeplyNested()
        {
            // Arrange
            string nestedCode = string.Join("", Enumerable.Repeat("{", 21)) +
                                string.Join("", Enumerable.Repeat("}", 21));
            string code = $"public void Test() {{ {nestedCode} }}";

            // Act
            List<string> issues = ReadabilityHelper.AnalyzeReadability(code);

            // Assert
            Assert.Contains(issues, issue => issue.Contains("Deeply nested structures detected"));
        }

        [Fact]
        public void AnalyzeReadability_ShouldIdentifyTodoComments()
        {
            // Arrange - Använd stora bokstäver för "TODO" för att matcha mot ToUpper i koden
            string code = "public void Test() { // TODO: Fix this later\nint x = 5; }";

            // Act
            List<string> issues = ReadabilityHelper.AnalyzeReadability(code);

            // Assert - Använd Contains istället för att söka efter exakt matchning
            Assert.Contains(issues, issue => issue.Contains("TODO comments found"));
        }

        [Fact]
        public void AnalyzeReadability_ShouldNotFlagCleanCode()
        {
            // Arrange - verkligt ren kod med alla element som krävs för att undvika alla kontroller
            string code = @"
            // This is a well-formatted function with good conventions
            public void TestFunction(int parameter) 
            { 
                int camelCaseVar = 10;
                String PascalCaseType = ""Test"";
                
                if (parameter > 0) {
                    Console.WriteLine(""Positive"");
                } else {
                    Console.WriteLine(""Non-positive"");
                }
            }";

            // Act
            List<string> issues = ReadabilityHelper.AnalyzeReadability(code);

            // Assert
            Assert.Empty(issues);
        }
    }
}