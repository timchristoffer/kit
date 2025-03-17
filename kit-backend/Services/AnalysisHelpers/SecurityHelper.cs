namespace KitBackend.Services.AnalysisHelpers
{
    public static class SecurityHelper
    {
        public static int CalculateSecurityScore(string code)
        {
            int score = 100;
            if (code.Contains("SELECT * FROM")) score -= 20;
            if (code.Contains("<script>") || code.Contains("document.write")) score -= 25;
            if (code.Contains("HttpClient") && code.Contains(".GetAsync")) score -= 15;
            if (code.Contains("eval")) score -= 30;
            if (code.Contains("Thread.Sleep")) score -= 20;
            if (code.Contains("password")) score -= 30;
            if (code.Contains("private") && code.Contains("string") && code.Contains("=")) score -= 20;
            if (code.Contains("MD5") || code.Contains("SHA1")) score -= 20;
            if (code.Contains("catch") && !code.Contains("throw")) score -= 10;
            if (code.Contains("HttpClient") && !code.Contains("Dispose")) score -= 10;
            if (code.Contains("File") && code.Contains(".ReadAllText")) score -= 15;
            if (code.Contains("File") && code.Contains(".WriteAllText")) score -= 15;
            if (code.Contains("Directory") && code.Contains(".CreateDirectory")) score -= 10;
            if (code.Contains("Directory") && code.Contains(".Delete")) score -= 10;

            return Math.Max(0, score);
        }

        public static List<string> PerformSecurityAnalysis(string code)
        {
            var issues = new List<string>();

            if (code.Contains("SELECT * FROM"))
            {
                issues.Add("Potential SQL Injection vulnerability detected: Avoid using SELECT * in queries. Consider specifying columns to select.");
            }

            if (code.Contains("<script>") || code.Contains("document.write"))
            {
                issues.Add("Potential XSS vulnerability detected: Avoid directly injecting JavaScript.");
            }

            if (code.Contains("HttpClient") && code.Contains(".GetAsync"))
            {
                issues.Add("Potential insecure HTTP call detected: Use HTTPS and ensure proper validation.");
            }

            if (code.Contains("eval"))
            {
                issues.Add("Potential risk of code injection detected: Avoid using eval to execute code.");
            }

            if (code.Contains("Thread.Sleep"))
            {
                issues.Add("Potential denial of service (DoS) attack: Avoid using Thread.Sleep, consider using async/await.");
            }

            if (code.Contains("password"))
            {
                issues.Add("Security risk: Hardcoded password detected. Avoid hardcoding passwords in the code.");
            }

            if (code.Contains("private") && code.Contains("string") && code.Contains("="))
            {
                issues.Add("Security risk: Hardcoded sensitive data detected. Avoid hardcoding sensitive data in the code.");
            }

            if (code.Contains("MD5") || code.Contains("SHA1"))
            {
                issues.Add("Security risk: Insecure cryptographic practice detected. Avoid using MD5 or SHA1.");
            }

            if (code.Contains("catch") && !code.Contains("throw"))
            {
                issues.Add("Security risk: Improper error handling detected. Ensure exceptions are properly handled.");
            }

            if (code.Contains("HttpClient") && !code.Contains("Dispose"))
            {
                issues.Add("Security risk: HttpClient instance not disposed properly.");
            }

            if (code.Contains("File") && code.Contains(".ReadAllText"))
            {
                issues.Add("Security risk: Potential file read vulnerability detected.");
            }

            if (code.Contains("File") && code.Contains(".WriteAllText"))
            {
                issues.Add("Security risk: Potential file write vulnerability detected.");
            }

            if (code.Contains("Directory") && code.Contains(".CreateDirectory"))
            {
                issues.Add("Security risk: Potential directory creation vulnerability detected.");
            }

            if (code.Contains("Directory") && code.Contains(".Delete"))
            {
                issues.Add("Security risk: Potential directory deletion vulnerability detected.");
            }

            return issues;
        }
    }
}
