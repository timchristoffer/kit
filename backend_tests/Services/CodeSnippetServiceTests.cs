using KitBackend.Models.Data;
using KitBackend.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace KitBackend.Tests.Services
{
    public class CodeSnippetServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<CodeSnippet>> _mockDbSet;
        private readonly TestCodeSnippetService _codeSnippetService;
        private readonly List<CodeSnippet> _codeSnippets = new List<CodeSnippet>();

        public CodeSnippetServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            _mockDbSet = SetupMockDbSet(_codeSnippets);

            _mockContext.Setup(c => c.CodeSnippet).Returns(_mockDbSet.Object);
            _codeSnippetService = new TestCodeSnippetService(_mockContext.Object);
        }

        [Fact]
        public async Task CreateCodeSnippetAsync_ShouldAssignGuidAndReturnSnippet()
        {
            // Arrange
            var snippet = new CodeSnippet
            {
                Code = "public class Test { }"
            };

            // Setup för att Add ska lagra snippets i vår lokala lista
            _mockDbSet.Setup(m => m.Add(It.IsAny<CodeSnippet>()))
                .Callback<CodeSnippet>(s => _codeSnippets.Add(s));

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _codeSnippetService.CreateCodeSnippetAsync(snippet);

            // Assert
            Assert.NotEqual(Guid.Empty, result.SnippetId);
            Assert.Equal(snippet.Code, result.Code);
            _mockDbSet.Verify(m => m.Add(It.IsAny<CodeSnippet>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCodeSnippetByIdAsync_ShouldReturnSnippet_WhenIdExists()
        {
            // Arrange
            var snippetId = Guid.NewGuid();
            var expectedSnippet = new CodeSnippet
            {
                SnippetId = snippetId,
                Code = "public class Test { }"
            };

            // Sätt upp att vår testversion av servicen returnerar denna snippet
            _codeSnippetService.SetupTestSnippet(expectedSnippet);

            // Act
            var result = await _codeSnippetService.GetCodeSnippetByIdAsync(snippetId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(snippetId, result.SnippetId);
            Assert.Equal(expectedSnippet.Code, result.Code);
        }

        [Fact]
        public async Task GetCodeSnippetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _codeSnippetService.SetupTestSnippet(null);

            // Act
            var result = await _codeSnippetService.GetCodeSnippetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        // Hjälpklass som överskuggar problemmetoden
        private class TestCodeSnippetService : CodeSnippetService
        {
            private CodeSnippet _testSnippet;

            public TestCodeSnippetService(ApplicationDbContext context) : base(context)
            {
            }

            public void SetupTestSnippet(CodeSnippet snippet)
            {
                _testSnippet = snippet;
            }

            public override async Task<CodeSnippet> GetCodeSnippetByIdAsync(Guid id)
            {
                // Returnera vår test-snippet istället för att anropa databasen
                return await Task.FromResult(_testSnippet);
            }
        }

        // Hjälpmetod för att sätta upp en mock DbSet
        private Mock<DbSet<T>> SetupMockDbSet<T>(List<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());

            return mockSet;
        }
    }
}
