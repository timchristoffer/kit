using System;
using System.Threading.Tasks;
using KitBackend.Models.Data;

namespace KitBackend.Services
{
    public interface ICodeSnippetService
    {
        Task<CodeSnippet> CreateCodeSnippetAsync(CodeSnippet snippet);
        Task<CodeSnippet> GetCodeSnippetByIdAsync(Guid id);
    }
}


