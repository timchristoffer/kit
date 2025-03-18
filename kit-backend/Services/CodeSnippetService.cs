using System;
using System.Threading.Tasks;
using KitBackend.Models.Data;

namespace KitBackend.Services
{
    public class CodeSnippetService : ICodeSnippetService
    {
        private readonly ApplicationDbContext _context;

        public CodeSnippetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<CodeSnippet> CreateCodeSnippetAsync(CodeSnippet snippet)
        {
            snippet.SnippetId = Guid.NewGuid();
            _context.CodeSnippet.Add(snippet);
            await _context.SaveChangesAsync();
            return snippet;
        }

        public virtual async Task<CodeSnippet> GetCodeSnippetByIdAsync(Guid id)
        {
            return await _context.CodeSnippet.FindAsync(id);
        }
    }
}

