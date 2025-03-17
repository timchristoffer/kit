namespace KitBackend.Services.AnalysisHelpers
{
    public static class PerformanceHelper
    {
        public static int CalculatePerformanceScore(string code)
        {
            int score = 100;
            if (code.Contains("Thread.Sleep")) score -= 20;
            if (code.Contains("lock")) score -= 15;
            if (code.Contains(".GetAsync") && !code.Contains("await")) score -= 25;
            if (code.Contains("foreach") && code.Contains("List") && code.Contains("ToList")) score -= 10;
            if (code.Contains("new") && code.Contains("List") && code.Contains("Capacity")) score -= 10;
            if (code.Contains("for") && code.Contains("Count")) score -= 10;
            if (code.Contains("new") && code.Contains("object")) score -= 10;
            if (code.Contains("Dictionary") && code.Contains("new string")) score -= 15;
            if (code.Contains("for") && code.Contains("for")) score -= 15;
            if (code.Contains("Console.WriteLine") && code.Contains("for")) score -= 10;
            if (code.Contains("List") && code.Contains("Add")) score -= 10;
            if (code.Contains("Dictionary") && code.Contains("Add")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Resize")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Copy")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Sort")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Reverse")) score -= 10;
            if (code.Contains("Array") && code.Contains(".IndexOf")) score -= 10;
            if (code.Contains("Array") && code.Contains(".LastIndexOf")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Find")) score -= 10;
            if (code.Contains("Array") && code.Contains(".FindAll")) score -= 10;
            if (code.Contains("Array") && code.Contains(".FindIndex")) score -= 10;
            if (code.Contains("Array") && code.Contains(".FindLast")) score -= 10;
            if (code.Contains("Array") && code.Contains(".FindLastIndex")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Exists")) score -= 10;
            if (code.Contains("Array") && code.Contains(".TrueForAll")) score -= 10;
            if (code.Contains("Array") && code.Contains(".ForEach")) score -= 10;
            if (code.Contains("Array") && code.Contains(".ConvertAll")) score -= 10;
            if (code.Contains("Array") && code.Contains(".BinarySearch")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Clear")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Clone")) score -= 10;
            if (code.Contains("Array") && code.Contains(".CopyTo")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetEnumerator")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetLength")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetLongLength")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetLowerBound")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetUpperBound")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Initialize")) score -= 10;
            if (code.Contains("Array") && code.Contains(".SetValue")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetValue")) score -= 10;

            return Math.Max(0, score);
        }

        public static List<string> PerformPerformanceAnalysis(string code)
        {
            var issues = new List<string>();

            if (code.Contains("Thread.Sleep"))
            {
                issues.Add("Potential performance issue: Avoid using Thread.Sleep, consider using async/await.");
            }

            if (code.Contains("lock"))
            {
                issues.Add("Performance warning: Ensure locks are scoped appropriately to avoid deadlocks.");
            }

            if (code.Contains("foreach") && code.Contains("List") && code.Contains("ToList"))
            {
                issues.Add("Performance issue: Avoid unnecessary ToList conversions when iterating over a collection.");
            }

            if (code.Contains("new") && code.Contains("List") && code.Contains("Capacity"))
            {
                issues.Add("Potential performance issue: Avoid excessive memory allocation by specifying an appropriate capacity for lists.");
            }

            if (code.Contains("for") && code.Contains("Count"))
            {
                issues.Add("Performance issue: Avoid inefficient loops by caching the count value.");
            }

            if (code.Contains("new") && code.Contains("object"))
            {
                issues.Add("Performance issue: Avoid unnecessary object creation.");
            }

            if (code.Contains("Dictionary") && code.Contains("new string"))
            {
                issues.Add("Performance issue: Excessive memory usage in dictionaries detected. Consider optimizing memory usage.");
            }

            if (code.Contains("for") && code.Contains("for"))
            {
                issues.Add("Performance issue: Nested loops detected. Consider optimizing the loop structure.");
            }

            if (code.Contains("Console.WriteLine") && code.Contains("for"))
            {
                issues.Add("Performance issue: Excessive console output in loops detected. Consider reducing console output.");
            }

            if (code.Contains("Array") && code.Contains(".Resize"))
            {
                issues.Add("Performance issue: Avoid using Array.Resize, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Copy"))
            {
                issues.Add("Performance issue: Avoid using Array.Copy, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Sort"))
            {
                issues.Add("Performance issue: Avoid using Array.Sort, consider using a more efficient sorting algorithm.");
            }

            if (code.Contains("Array") && code.Contains(".Reverse"))
            {
                issues.Add("Performance issue: Avoid using Array.Reverse, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".IndexOf"))
            {
                issues.Add("Performance issue: Avoid using Array.IndexOf, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".LastIndexOf"))
            {
                issues.Add("Performance issue: Avoid using Array.LastIndexOf, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Find"))
            {
                issues.Add("Performance issue: Avoid using Array.Find, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".FindAll"))
            {
                issues.Add("Performance issue: Avoid using Array.FindAll, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".FindIndex"))
            {
                issues.Add("Performance issue: Avoid using Array.FindIndex, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".FindLast"))
            {
                issues.Add("Performance issue: Avoid using Array.FindLast, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".FindLastIndex"))
            {
                issues.Add("Performance issue: Avoid using Array.FindLastIndex, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Exists"))
            {
                issues.Add("Performance issue: Avoid using Array.Exists, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".TrueForAll"))
            {
                issues.Add("Performance issue: Avoid using Array.TrueForAll, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".ForEach"))
            {
                issues.Add("Performance issue: Avoid using Array.ForEach, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".ConvertAll"))
            {
                issues.Add("Performance issue: Avoid using Array.ConvertAll, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".BinarySearch"))
            {
                issues.Add("Performance issue: Avoid using Array.BinarySearch, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Clear"))
            {
                issues.Add("Performance issue: Avoid using Array.Clear, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Clone"))
            {
                issues.Add("Performance issue: Avoid using Array.Clone, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".CopyTo"))
            {
                issues.Add("Performance issue: Avoid using Array.CopyTo, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetEnumerator"))
            {
                issues.Add("Performance issue: Avoid using Array.GetEnumerator, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetLength"))
            {
                issues.Add("Performance issue: Avoid using Array.GetLength, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetLongLength"))
            {
                issues.Add("Performance issue: Avoid using Array.GetLongLength, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetLowerBound"))
            {
                issues.Add("Performance issue: Avoid using Array.GetLowerBound, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetUpperBound"))
            {
                issues.Add("Performance issue: Avoid using Array.GetUpperBound, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Initialize"))
            {
                issues.Add("Performance issue: Avoid using Array.Initialize, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".SetValue"))
            {
                issues.Add("Performance issue: Avoid using Array.SetValue, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetValue"))
            {
                issues.Add("Performance issue: Avoid using Array.GetValue, consider using a more efficient data structure.");
            }

            return issues;
        }
    }
}
