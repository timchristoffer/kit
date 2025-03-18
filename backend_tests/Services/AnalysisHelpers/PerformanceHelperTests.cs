using KitBackend.Services.AnalysisHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace KitBackend.Tests.Services.AnalysisHelpers
{
    public class PerformanceHelperTests
    {
        [Theory]
        [InlineData("Thread.Sleep(1000);", 80)]
        [InlineData("lock (obj) { }", 85)]
        [InlineData("var result = client.GetAsync(url);", 75)]
        [InlineData("foreach (var item in list.ToList()) { }", 60)]
        [InlineData("var list = new List<int>(capacity);", 100)]
        [InlineData("for (int i = 0; i < list.Count; i++) { }", 60)]
        [InlineData("var obj = new object();", 90)]
        [InlineData("var dict = new Dictionary<string, string>();", 85)]
        [InlineData("for (int i = 0; i < 10; i++) { for (int j = 0; j < 10; j++) { } }", 85)]
        [InlineData("for (int i = 0; i < 10; i++) { Console.WriteLine(i); }", 60)]
        [InlineData("var list = new List<int>(); list.Add(1);", 90)]
        [InlineData("var dict = new Dictionary<int, int>(); dict.Add(1, 1);", 90)]
        [InlineData("Array.Resize(ref array, newSize);", 90)]
        [InlineData("Array.Copy(sourceArray, destinationArray, length);", 90)]
        [InlineData("Array.Sort(array);", 90)]
        [InlineData("Array.Reverse(array);", 90)]
        [InlineData("Array.IndexOf(array, value);", 90)]
        [InlineData("Array.LastIndexOf(array, value);", 90)]
        [InlineData("Array.Find(array, match);", 90)]
        [InlineData("Array.FindAll(array, match);", 70)]
        [InlineData("Array.FindIndex(array, match);", 70)]
        [InlineData("Array.FindLast(array, match);", 70)]
        [InlineData("Array.FindLastIndex(array, match);", 40)]
        [InlineData("Array.Exists(array, match);", 90)]
        [InlineData("Array.TrueForAll(array, match);", 90)]
        [InlineData("Array.ForEach(array, action);", 90)]
        [InlineData("Array.ConvertAll(array, converter);", 90)]
        [InlineData("Array.BinarySearch(array, value);", 90)]
        [InlineData("Array.Clear(array, index, length);", 90)]
        [InlineData("Array.Clone();", 90)]
        [InlineData("Array.CopyTo(array, index);", 70)]
        [InlineData("Array.GetEnumerator();", 90)]
        [InlineData("Array.GetLength(dimension);", 90)]
        [InlineData("Array.GetLongLength(dimension);", 90)]
        [InlineData("Array.GetLowerBound(dimension);", 90)]
        [InlineData("Array.GetUpperBound(dimension);", 90)]
        [InlineData("Array.Initialize();", 90)]
        [InlineData("Array.SetValue(value, index);", 90)]
        [InlineData("Array.GetValue(index);", 90)]
        public void CalculatePerformanceScore_ShouldDecrease_ForPerformanceIssues(string code, int expectedScore)
        {
            // Act
            int score = PerformanceHelper.CalculatePerformanceScore(code);

            // Assert
            Assert.Equal(expectedScore, score);
        }

        [Theory]
        [InlineData("Thread.Sleep(1000);", "Potential performance issue: Avoid using Thread.Sleep, consider using async/await.")]
        [InlineData("lock (obj) { }", "Performance warning: Ensure locks are scoped appropriately to avoid deadlocks.")]
        [InlineData("foreach (var item in list.ToList()) { }", "Performance issue: Avoid unnecessary ToList conversions when iterating over a collection.")]
        [InlineData("var list = new List<int>(capacity);", "Potential performance issue: Avoid excessive memory allocation by specifying an appropriate capacity for lists.")]
        [InlineData("for (int i = 0; i < list.Count; i++) { }", "Performance issue: Avoid inefficient loops by caching the count value.")]
        [InlineData("var obj = new object();", "Performance issue: Avoid unnecessary object creation.")]
        [InlineData("var dict = new Dictionary<string, string>();", "Performance issue: Excessive memory usage in dictionaries detected. Consider optimizing memory usage.")]
        [InlineData("for (int i = 0; i < 10; i++) { for (int j = 0; j < 10; j++) { } }", "Performance issue: Nested loops detected. Consider optimizing the loop structure.")]
        [InlineData("for (int i = 0; i < 10; i++) { Console.WriteLine(i); }", "Performance issue: Excessive console output in loops detected. Consider reducing console output.")]
        [InlineData("Array.Resize(ref array, newSize);", "Performance issue: Avoid using Array.Resize, consider using a more efficient data structure.")]
        [InlineData("Array.Copy(sourceArray, destinationArray, length);", "Performance issue: Avoid using Array.Copy, consider using a more efficient data structure.")]
        [InlineData("Array.Sort(array);", "Performance issue: Avoid using Array.Sort, consider using a more efficient sorting algorithm.")]
        [InlineData("Array.Reverse(array);", "Performance issue: Avoid using Array.Reverse, consider using a more efficient data structure.")]
        [InlineData("Array.IndexOf(array, value);", "Performance issue: Avoid using Array.IndexOf, consider using a more efficient data structure.")]
        [InlineData("Array.LastIndexOf(array, value);", "Performance issue: Avoid using Array.LastIndexOf, consider using a more efficient data structure.")]
        [InlineData("Array.Find(array, match);", "Performance issue: Avoid using Array.Find, consider using a more efficient data structure.")]
        [InlineData("Array.FindAll(array, match);", "Performance issue: Avoid using Array.FindAll, consider using a more efficient data structure.")]
        [InlineData("Array.FindIndex(array, match);", "Performance issue: Avoid using Array.FindIndex, consider using a more efficient data structure.")]
        [InlineData("Array.FindLast(array, match);", "Performance issue: Avoid using Array.FindLast, consider using a more efficient data structure.")]
        [InlineData("Array.FindLastIndex(array, match);", "Performance issue: Avoid using Array.FindLastIndex, consider using a more efficient data structure.")]
        [InlineData("Array.Exists(array, match);", "Performance issue: Avoid using Array.Exists, consider using a more efficient data structure.")]
        [InlineData("Array.TrueForAll(array, match);", "Performance issue: Avoid using Array.TrueForAll, consider using a more efficient data structure.")]
        [InlineData("Array.ForEach(array, action);", "Performance issue: Avoid using Array.ForEach, consider using a more efficient data structure.")]
        [InlineData("Array.ConvertAll(array, converter);", "Performance issue: Avoid using Array.ConvertAll, consider using a more efficient data structure.")]
        [InlineData("Array.BinarySearch(array, value);", "Performance issue: Avoid using Array.BinarySearch, consider using a more efficient data structure.")]
        [InlineData("Array.Clear(array, index, length);", "Performance issue: Avoid using Array.Clear, consider using a more efficient data structure.")]
        [InlineData("Array.Clone();", "Performance issue: Avoid using Array.Clone, consider using a more efficient data structure.")]
        [InlineData("Array.CopyTo(array, index);", "Performance issue: Avoid using Array.CopyTo, consider using a more efficient data structure.")]
        [InlineData("Array.GetEnumerator();", "Performance issue: Avoid using Array.GetEnumerator, consider using a more efficient data structure.")]
        [InlineData("Array.GetLength(dimension);", "Performance issue: Avoid using Array.GetLength, consider using a more efficient data structure.")]
        [InlineData("Array.GetLongLength(dimension);", "Performance issue: Avoid using Array.GetLongLength, consider using a more efficient data structure.")]
        [InlineData("Array.GetLowerBound(dimension);", "Performance issue: Avoid using Array.GetLowerBound, consider using a more efficient data structure.")]
        [InlineData("Array.GetUpperBound(dimension);", "Performance issue: Avoid using Array.GetUpperBound, consider using a more efficient data structure.")]
        [InlineData("Array.Initialize();", "Performance issue: Avoid using Array.Initialize, consider using a more efficient data structure.")]
        [InlineData("Array.SetValue(value, index);", "Performance issue: Avoid using Array.SetValue, consider using a more efficient data structure.")]
        [InlineData("Array.GetValue(index);", "Performance issue: Avoid using Array.GetValue, consider using a more efficient data structure.")]
        [InlineData("var list = new List<int>(capacity);", "Potential performance issue: Avoid excessive memory allocation by specifying an appropriate capacity for lists.")]
        [InlineData("var dict = new Dictionary<string, string>();", "Performance issue: Excessive memory usage in dictionaries detected. Consider optimizing memory usage.")]
        public void PerformPerformanceAnalysis_ShouldIdentifyPerformanceIssues(string code, string expectedIssue)
        {
            // Act
            List<string> issues = PerformanceHelper.PerformPerformanceAnalysis(code);

            // Assert
            Assert.Contains(expectedIssue, issues);
        }
    }
}
