using Microsoft.ML.Data;

namespace KitBackend.Models.Data
{
    public class CodeAnalysisPrediction
    {
        [ColumnName("PredictedLabel")]
        public int Score { get; set; }
        public string Explanation { get; set; }
    }
}