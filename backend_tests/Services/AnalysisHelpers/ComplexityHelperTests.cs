using KitBackend.Services.AnalysisHelpers;
using System;
using Xunit;

namespace KitBackend.Tests.Services.AnalysisHelpers
{
    public class ComplexityHelperTests
    {
        [Fact]
        public void CalculateComplexityScore_ShouldReturn100_ForEmptyCode()
        {
            // Arrange
            string code = string.Empty;

            // Act
            int score = ComplexityHelper.CalculateComplexityScore(code);

            // Assert
            Assert.Equal(100, score);
        }

        [Fact]
        public void CalculateComplexityScore_ShouldDecrease_ForHighCyclomaticComplexity()
        {
            // Arrange
            string code = @"
                public void ComplexMethod() {
                    if (condition1) {
                        // do something
                    }
                    if (condition2) {
                        // do something
                    }
                    if (condition3) {
                        // do something
                    }
                    if (condition4) {
                        // do something
                    }
                    if (condition5) {
                        // do something
                    }
                    if (condition6) {
                        // do something
                    }
                    if (condition7) {
                        // do something
                    }
                    if (condition8) {
                        // do something
                    }
                    if (condition9) {
                        // do something
                    }
                    if (condition10) {
                        // do something
                    }
                    if (condition11) {
                        // do something
                    }
                }";

            // Act
            int score = ComplexityHelper.CalculateComplexityScore(code);

            // Assert
            Assert.True(score < 100);
            Assert.Equal(80, score); // 100 - 20 for high cyclomatic complexity
        }

        [Fact]
        public void CalculateComplexityScore_ShouldDecrease_ForManyClassMembers()
        {
            // Arrange
            string code = @"
                public void Method1() {}
                private void Method2() {}
                protected void Method3() {}
                public void Method4() {}
                private void Method5() {}
                protected void Method6() {}
                public void Method7() {}
                private void Method8() {}
                protected void Method9() {}
                public void Method10() {}
                private void Method11() {}
                protected void Method12() {}
                public void Method13() {}
                private void Method14() {}
                protected void Method15() {}
                public void Method16() {}
                private void Method17() {}
                protected void Method18() {}
                public void Method19() {}
                private void Method20() {}
                protected void Method21() {}";

            // Act
            int score = ComplexityHelper.CalculateComplexityScore(code);

            // Assert
            Assert.True(score < 100);
            Assert.Equal(90, score); // 100 - 10 for many class members
        }

        [Fact]
        public void CalculateComplexityScore_ShouldDecrease_ForInheritance()
        {
            // Arrange
            string code = "public class Child : Parent { }";

            // Act
            int score = ComplexityHelper.CalculateComplexityScore(code);

            // Assert
            Assert.Equal(90, score); // 100 - 10 for inheritance
        }

        [Theory]
        [InlineData("interface", "public interface IExample { }", 80)]
        [InlineData("abstract", "public abstract class AbstractClass { }", 80)]
        [InlineData("virtual", "public virtual void Method() { }", 80)]
        [InlineData("override", "public override void Method() { }", 80)]
        [InlineData("sealed", "public sealed class SealedClass { }", 80)]
        [InlineData("static", "public static void Method() { }", 80)]
        [InlineData("readonly", "private readonly int _field;", 80)]
        [InlineData("const", "private const int Constant = 10;", 80)]
        [InlineData("enum", "public enum DayOfWeek { Monday, Tuesday }", 80)]
        [InlineData("struct", "public struct Point { }", 80)]
        [InlineData("delegate", "public delegate void Handler();", 80)]
        [InlineData("event", "public event EventHandler Changed;", 80)]
        [InlineData("operator", "public static Point operator +(Point a, Point b) { }", 60)]
        [InlineData("implicit", "public static implicit operator int(MyClass v) { }", 20)]
        [InlineData("explicit", "public static explicit operator string(MyClass v) { }", 20)]
        [InlineData("params", "public void Method(params int[] numbers) { }", 80)]
        [InlineData("ref", "public void Method(ref int number) { }", 80)]
        [InlineData("out", "public void Method(out int number) { }", 80)]
        [InlineData("in", "public void Method(in int number) { }", 80)]
        [InlineData("yield", "public IEnumerable<int> Method() { yield return 1; }", 40)]
        [InlineData("async", "public async Task Method() { }", 80)]
        [InlineData("await", "await Task.Delay(1000);", 80)]
        [InlineData("lock", "lock (obj) { }", 80)]
        [InlineData("using", "using (var resource = new Resource()) { }", 80)]
        [InlineData("fixed", "fixed (int* p = &i) { }", 80)]
        [InlineData("unsafe", "unsafe { int* ptr = &i; }", 80)]
        [InlineData("checked", "checked { int result = a + b; }", 80)]
        [InlineData("unchecked", "unchecked { int result = a + b; }", 60)]
        [InlineData("sizeof", "int size = sizeof(int);", 80)]
        [InlineData("typeof", "Type type = typeof(string);", 80)]
        [InlineData("nameof", "string name = nameof(variable);", 80)]
        [InlineData("default", "int value = default(int);", 80)]
        [InlineData("switch", "switch (value) { }", 80)]
        [InlineData("case", "case 1: break;", 60)]
        [InlineData("goto", "goto label;", 80)]
        [InlineData("continue", "continue;", 80)]
        [InlineData("break", "break;", 80)]
        [InlineData("try", "try { }", 80)]
        [InlineData("catch", "catch (Exception ex) { }", 80)]
        [InlineData("finally", "finally { }", 80)]
        [InlineData("throw", "throw new Exception();", 80)]
        [InlineData("return", "return value;", 80)]
        public void CalculateComplexityScore_ShouldDecrease_ForComplexityFeatures(string feature, string code, int expectedScore)
        {
            // Använd feature-parametern för att undvika xUnit1026-varningen
            Console.WriteLine($"Testing complexity feature: {feature}");

            // Act
            int score = ComplexityHelper.CalculateComplexityScore(code);

            // Assert
            Assert.True(score < 100, $"Feature '{feature}' should decrease complexity score below 100");
            Assert.Equal(expectedScore, score);
        }

        [Fact]
        public void CalculateComplexityScore_ShouldDecrease_ForMultipleComplexityFeatures()
        {
            // Arrange
            string code = @"
                public class Child : Parent {
                    private readonly int _field;
                    public static void Method() {
                        try {
                            if (condition) {
                                throw new Exception();
                            }
                        }
                        catch (Exception ex) {
                            // Handle error
                        }
                        finally {
                            // Clean up
                        }
                    }
                }";

            // Act
            int score = ComplexityHelper.CalculateComplexityScore(code);

            // Assert
            Assert.True(score <= 30); // Nu borde poängen vara ännu lägre med ny implementation
        }

        [Fact]
        public void CalculateComplexityScore_ShouldReturnZero_ForExcessivelyComplexCode()
        {
            // Arrange
            // Create a string with all complexity features
            var complexFeatures = new[] {
                "class", "interface", "abstract", "virtual", "override", "sealed", "static",
                "readonly", "const", "enum", "struct", "delegate", "event", "operator",
                "implicit", "explicit", "params", "ref", "out", "in", "yield", "async",
                "await", "lock", "using", "fixed", "unsafe", "checked", "unchecked", "sizeof",
                "typeof", "nameof", "default", "switch", "case", "goto", "continue", "break",
                "try", "catch", "finally", "throw", "return"
            };

            string code = string.Join(" ", complexFeatures) + " : " + string.Join(" ", complexFeatures);

            // Add high cyclomatic complexity
            for (int i = 0; i < 100; i++)
            {
                code += " if (condition) { }";
            }

            // Act
            int score = ComplexityHelper.CalculateComplexityScore(code);

            // Assert
            Assert.Equal(0, score); // Score should be at minimum (0)
        }
    }
}


