using System;
using System.Threading.Tasks;
using KitBackend.Models.Requests;
using KitBackend.Models.Responses;

namespace KitBackend.Services
{
    public interface IAnalysisService
    {
        Task<AnalysisReport> GenerateReportAsync(string code);
        Task<AnalysisReport> GetReportByIdAsync(Guid id);
        Task SaveAnalysisStatusAsync(AnalysisStatus status);
        Task UpdateAnalysisStatusAsync(AnalysisStatus status);
        Task UpdateReportAsync(AnalysisReport report);
    }
}