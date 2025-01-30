using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KitBackend.Models.Data;
using Microsoft.Extensions.Logging;

public class CodeAnalysisPrediction
{
    [ColumnName("PredictedLabel")]
    public string Explanation { get; set; }
}

public class MLModelService
{
    private readonly MLContext _mlContext;
    private ITransformer _model;
    private List<CodeAnalysisData> _trainingData;
    private readonly ILogger<MLModelService> _logger;

    public MLModelService(ILogger<MLModelService> logger)
    {
        _mlContext = new MLContext();
        _trainingData = LoadTrainingData().ToList();
        _logger = logger;
        TrainModel();
    }

    private void TrainModel()
    {
        try
        {
            var trainingDataView = _mlContext.Data.LoadFromEnumerable(_trainingData);

            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(CodeAnalysisData.Code))
                .Append(_mlContext.Transforms.Concatenate("Features", "Features"))
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(CodeAnalysisData.Explanation)))
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            _model = pipeline.Fit(trainingDataView);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training the model.");
            throw;
        }
    }

    public void AddTrainingData(CodeAnalysisData newTrainingData)
    {
        _trainingData.Add(newTrainingData);
        // Train the model in batches or periodically to improve performance
        if (_trainingData.Count % 10 == 0) // Example: Train every 10 new data points
        {
            TrainModel();
        }
    }

    public Dictionary<string, string> Predict(string code)
    {
        try
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<CodeAnalysisData, CodeAnalysisPrediction>(_model);
            var prediction = predictionEngine.Predict(new CodeAnalysisData { Code = code });

            // Log the prediction for debugging
            _logger.LogInformation($"Prediction: {prediction.Explanation}");

            // Split the explanation into categories
            var explanationDict = new Dictionary<string, string>
            {
                { "Readability", ExtractExplanation(prediction.Explanation, "Readability") },
                { "Performance", ExtractExplanation(prediction.Explanation, "Performance") },
                { "Security", ExtractExplanation(prediction.Explanation, "Security") },
                { "Complexity", ExtractExplanation(prediction.Explanation, "Complexity") }
            };

            return explanationDict;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error predicting the code analysis.");
            throw;
        }
    }

    private string ExtractExplanation(string explanation, string category)
    {
        // Define a regex pattern to match the category explanation
        var pattern = $@"{category}:\s*(.*?)(?=\s*(Readability|Performance|Security|Complexity|$))";
        var match = Regex.Match(explanation, pattern, RegexOptions.Singleline);

        // Return the matched explanation or a default message if not found
        return match.Success ? match.Groups[1].Value.Trim() : $"No specific {category} issues found.";
    }

    private IEnumerable<CodeAnalysisData> LoadTrainingData()
    {
        return new List<CodeAnalysisData>
        {
            new CodeAnalysisData
            {
                Code = "public class Simple { public void Method() { int x = 1; } }",
                ComplexityScore = 2,
                ReadabilityScore = 9,
                SecurityScore = 10,
                PerformanceScore = 10,
                Explanation = "Readability: This is a simple class with a single method. The code is easy to read and has no security or performance issues."
            },
            new CodeAnalysisData
            {
                Code = "public class Complex { public void Method() { for(int i = 0; i < 100; i++) { if(i % 2 == 0) { Console.WriteLine(i); } } } }",
                ComplexityScore = 8,
                ReadabilityScore = 5,
                SecurityScore = 10,
                PerformanceScore = 7,
                Explanation = "Complexity: This class contains a method with a nested loop and conditional statements, making it more complex. Readability: The readability is lower due to the nested structures."
            },
            new CodeAnalysisData
            {
                Code = "public class SecurityIssue { public void Method() { string password = \"12345\"; } }",
                ComplexityScore = 2,
                ReadabilityScore = 8,
                SecurityScore = 3,
                PerformanceScore = 10,
                Explanation = "Security: This class contains hardcoded sensitive data, which is a security risk. Avoid hardcoding passwords in the code. Hardcoded passwords can be easily extracted and used by malicious actors to gain unauthorized access."
            },
            new CodeAnalysisData
            {
                Code = "public class PerformanceIssue { public void Method() { Thread.Sleep(1000); } }",
                ComplexityScore = 2,
                ReadabilityScore = 8,
                SecurityScore = 10,
                PerformanceScore = 3,
                Explanation = "Performance: This class contains a method that uses Thread.Sleep, which can cause performance issues. Consider using async/await instead. Thread.Sleep blocks the current thread, which can lead to poor performance and unresponsive applications."
            },
            new CodeAnalysisData
            {
                Code = "public class ReadabilityIssue { public void Method() { int x = 1; int y = 2; int z = x + y; Console.WriteLine(z); } }",
                ComplexityScore = 2,
                ReadabilityScore = 4,
                SecurityScore = 10,
                PerformanceScore = 10,
                Explanation = "Readability: This class contains a method with unclear variable names and lack of comments, making it harder to read and understand. Clear and descriptive variable names and comments improve code readability and maintainability."
            },
            // Add more training data
        };
    }
}