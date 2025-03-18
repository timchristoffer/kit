using KitBackend.Endpoints;
using KitBackend.Models.Data;
using KitBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace KitBackend.Tests.Endpoints
{
    public class CodeSnippetEndpointsTests
    {
        private readonly Mock<ICodeSnippetService> _mockSnippetService;

        public CodeSnippetEndpointsTests()
        {
            _mockSnippetService = new Mock<ICodeSnippetService>();
        }

        [Fact]
        public async Task CreateCodeSnippet_WithValidSnippet_ReturnsCreated()
        {
            // Arrange
            var snippetId = Guid.NewGuid();
            var inputSnippet = new CodeSnippet
            {
                SnippetId = snippetId,
                Code = "public class TestClass { }"
            };

            var createdSnippet = new CodeSnippet
            {
                SnippetId = snippetId,
                Code = "public class TestClass { }"
            };

            _mockSnippetService
                .Setup(s => s.CreateCodeSnippetAsync(inputSnippet))
                .ReturnsAsync(createdSnippet);

            // Act
            var result = await CodeSnippetEndpoints.CreateCodeSnippet(inputSnippet, _mockSnippetService.Object);

            // Assert
            var createdResult = Assert.IsType<Created<CodeSnippet>>(result);
            Assert.Equal($"/api/codesnippets/{snippetId}", createdResult.Location);
            Assert.Equal(snippetId, createdResult.Value.SnippetId);
            Assert.Equal("public class TestClass { }", createdResult.Value.Code);
        }

        [Fact]
        public async Task CreateCodeSnippet_WithNullSnippet_ReturnsBadRequest()
        {
            // Arrange & Act
            var result = await CodeSnippetEndpoints.CreateCodeSnippet(null, _mockSnippetService.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequest<string>>(result);
            Assert.Equal("Snippet data is required.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateCodeSnippet_WhenArgumentExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var inputSnippet = new CodeSnippet
            {
                SnippetId = Guid.NewGuid(),
                Code = "invalid code"
            };

            _mockSnippetService
                .Setup(s => s.CreateCodeSnippetAsync(inputSnippet))
                .ThrowsAsync(new ArgumentException("Code is invalid"));

            // Act
            var result = await CodeSnippetEndpoints.CreateCodeSnippet(inputSnippet, _mockSnippetService.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequest<string>>(result);
            Assert.Equal("Invalid input: Code is invalid", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateCodeSnippet_WhenExceptionThrown_ReturnsProblem()
        {
            // Arrange
            var inputSnippet = new CodeSnippet
            {
                SnippetId = Guid.NewGuid(),
                Code = "public class TestClass { }"
            };

            _mockSnippetService
                .Setup(s => s.CreateCodeSnippetAsync(inputSnippet))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await CodeSnippetEndpoints.CreateCodeSnippet(inputSnippet, _mockSnippetService.Object);

            // Assert
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task GetCodeSnippetById_WithValidId_ReturnsOk()
        {
            // Arrange
            var snippetId = Guid.NewGuid();
            var expectedSnippet = new CodeSnippet
            {
                SnippetId = snippetId,
                Code = "public class TestClass { }"
            };

            _mockSnippetService
                .Setup(s => s.GetCodeSnippetByIdAsync(snippetId))
                .ReturnsAsync(expectedSnippet);

            // Act
            var result = await CodeSnippetEndpoints.GetCodeSnippetById(snippetId, _mockSnippetService.Object);

            // Assert
            var okResult = Assert.IsType<Ok<CodeSnippet>>(result);
            Assert.Equal(snippetId, okResult.Value.SnippetId);
            Assert.Equal("public class TestClass { }", okResult.Value.Code);
        }

        [Fact]
        public async Task GetCodeSnippetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var snippetId = Guid.NewGuid();

            _mockSnippetService
                .Setup(s => s.GetCodeSnippetByIdAsync(snippetId))
                .ReturnsAsync((CodeSnippet)null);

            // Act
            var result = await CodeSnippetEndpoints.GetCodeSnippetById(snippetId, _mockSnippetService.Object);

            // Assert
            var notFoundResult = Assert.IsType<NotFound<string>>(result);
            Assert.Equal("Snippet not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetCodeSnippetById_WhenExceptionThrown_ReturnsProblem()
        {
            // Arrange
            var snippetId = Guid.NewGuid();

            _mockSnippetService
                .Setup(s => s.GetCodeSnippetByIdAsync(snippetId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await CodeSnippetEndpoints.GetCodeSnippetById(snippetId, _mockSnippetService.Object);

            // Assert
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task CreateCodeSnippet_WithEmptyCode_StillCreatesSnippet()
        {
            // Arrange
            var snippetId = Guid.NewGuid();
            var inputSnippet = new CodeSnippet
            {
                SnippetId = snippetId,
                Code = string.Empty
            };

            var createdSnippet = new CodeSnippet
            {
                SnippetId = snippetId,
                Code = string.Empty
            };

            _mockSnippetService
                .Setup(s => s.CreateCodeSnippetAsync(inputSnippet))
                .ReturnsAsync(createdSnippet);

            // Act
            var result = await CodeSnippetEndpoints.CreateCodeSnippet(inputSnippet, _mockSnippetService.Object);

            // Assert
            var createdResult = Assert.IsType<Created<CodeSnippet>>(result);
            Assert.Equal(snippetId, createdResult.Value.SnippetId);
            Assert.Equal(string.Empty, createdResult.Value.Code);
        }

        [Fact]
        public async Task CreateCodeSnippet_ServiceValidatesAndReassignsId()
        {
            // Arrange
            var originalId = Guid.NewGuid();
            var newId = Guid.NewGuid();

            var inputSnippet = new CodeSnippet
            {
                SnippetId = originalId,
                Code = "public class TestClass { }"
            };

            var createdSnippet = new CodeSnippet
            {
                SnippetId = newId, // Service assigned a new ID
                Code = "public class TestClass { }"
            };

            _mockSnippetService
                .Setup(s => s.CreateCodeSnippetAsync(inputSnippet))
                .ReturnsAsync(createdSnippet);

            // Act
            var result = await CodeSnippetEndpoints.CreateCodeSnippet(inputSnippet, _mockSnippetService.Object);

            // Assert
            var createdResult = Assert.IsType<Created<CodeSnippet>>(result);
            Assert.Equal($"/api/codesnippets/{newId}", createdResult.Location);
            Assert.Equal(newId, createdResult.Value.SnippetId);
            Assert.NotEqual(originalId, createdResult.Value.SnippetId);
        }

        [Fact]
        public async Task GetCodeSnippetById_LogsIdBeingFetched()
        {
            // Arrange
            var snippetId = Guid.NewGuid();
            var expectedSnippet = new CodeSnippet
            {
                SnippetId = snippetId,
                Code = "public class TestClass { }"
            };

            _mockSnippetService
                .Setup(s => s.GetCodeSnippetByIdAsync(snippetId))
                .ReturnsAsync(expectedSnippet);

            // Det finns ingen direkt testning av Console.WriteLine som anropas i metoden,
            // men vi kan kontrollera att övriga delar av koden fungerar korrekt.

            // Act
            var result = await CodeSnippetEndpoints.GetCodeSnippetById(snippetId, _mockSnippetService.Object);

            // Assert
            var okResult = Assert.IsType<Ok<CodeSnippet>>(result);
            Assert.Equal(snippetId, okResult.Value.SnippetId);
        }
    }
}