namespace KitBackend.Models.Data
{
    public class CodeAnalysisData
    {
        public string Code { get; set; }
        public int ComplexityScore { get; set; }
        public int ReadabilityScore { get; set; }
        public int SecurityScore { get; set; }
        public int PerformanceScore { get; set; }
        public string Explanation { get; set; }
    }
    
}