using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KitBackend.Models.Data;
using KitBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Xunit;

namespace KitBackend.Tests.Services
{
    public class FileServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<UploadedFile>> _mockDbSet;
        private readonly FileService _fileService;

        public FileServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>(MockBehavior.Loose,
                new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            _mockDbSet = new Mock<DbSet<UploadedFile>>();

            _mockContext.Setup(c => c.UploadedFile).Returns(_mockDbSet.Object);
            _fileService = new FileService(_mockContext.Object);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldReturnNull_WhenFileIsNull()
        {
            // Arrange
            IFormFile? nullFile = null;
            string uploader = "testUser";

            // Act
            var result = await _fileService.UploadFileAsync(nullFile!, uploader);

            // Assert
            Assert.Null(result);
            _mockDbSet.Verify(m => m.Add(It.IsAny<UploadedFile>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldReturnNull_WhenFileIsEmpty()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0);
            string uploader = "testUser";

            // Act
            var result = await _fileService.UploadFileAsync(mockFile.Object, uploader);

            // Assert
            Assert.Null(result);
            _mockDbSet.Verify(m => m.Add(It.IsAny<UploadedFile>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldReturnNull_WhenFileExtensionIsNotAllowed()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(100);
            mockFile.Setup(f => f.FileName).Returns("test.pdf");
            string uploader = "testUser";

            // Act
            var result = await _fileService.UploadFileAsync(mockFile.Object, uploader);

            // Assert
            Assert.Null(result);
            _mockDbSet.Verify(m => m.Add(It.IsAny<UploadedFile>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldReturnUploadedFile_WhenFileIsValid()
        {
            // Arrange
            string fileName = "test.js";
            long fileSize = 100;
            string uploader = "testUser";
            byte[] fileContent = Encoding.UTF8.GetBytes("console.log('Hello, World!');");

            var mockFile = CreateMockFormFile(fileContent, fileName, fileSize);

            // Setup SaveChangesAsync to simulate Id being set after saving
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() =>
                {
                    // Simulate the ID being set after SaveChangesAsync
                    if (_mockDbSet.Invocations.Count > 0)
                    {
                        var capturedFile = (UploadedFile)_mockDbSet.Invocations[0].Arguments[0];
                        capturedFile.Id = 1;
                    }
                });

            // Act
            var result = await _fileService.UploadFileAsync(mockFile.Object, uploader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileName, result.FileName);
            Assert.Equal(fileSize, result.FileSize);
            Assert.Equal(uploader, result.Uploader);
            Assert.Equal(DateTime.UtcNow.Date, result.UploadDate.Date);
            Assert.Equal(fileContent, result.FileContent);

            _mockDbSet.Verify(m => m.Add(It.Is<UploadedFile>(
                f => f.FileName == fileName &&
                     f.FileSize == fileSize &&
                     f.Uploader == uploader)),
                Times.Once);

            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(".cs")]
        [InlineData(".js")]
        [InlineData(".jsx")]
        [InlineData(".ts")]
        [InlineData(".tsx")]
        [InlineData(".css")]
        public async Task UploadFileAsync_ShouldAcceptAllowedExtensions(string extension)
        {
            // Arrange
            string fileName = $"test{extension}";
            long fileSize = 100;
            string uploader = "testUser";
            byte[] fileContent = Encoding.UTF8.GetBytes("Sample content");

            var mockFile = CreateMockFormFile(fileContent, fileName, fileSize);

            // Setup SaveChangesAsync
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() =>
                {
                    // Simulate the ID being set after SaveChangesAsync
                    if (_mockDbSet.Invocations.Count > 0)
                    {
                        var capturedFile = (UploadedFile)_mockDbSet.Invocations[0].Arguments[0];
                        capturedFile.Id = 1;
                    }
                });

            // Act
            var result = await _fileService.UploadFileAsync(mockFile.Object, uploader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileName, result.FileName);
        }

        [Fact]
        public async Task GetFileById_ShouldReturnFile_WhenFileExists()
        {
            // Arrange
            int fileId = 1;
            var expectedFile = new UploadedFile
            {
                Id = fileId,
                FileName = "test.js",
                FileSize = 100,
                Uploader = "testUser",
                UploadDate = DateTime.UtcNow,
                FileContent = Encoding.UTF8.GetBytes("console.log('Hello, World!');")
            };

            // Mer precis mock - matcha exakt vad FindAsync anropas med
            _mockDbSet.Setup(d => d.FindAsync(fileId))
                .ReturnsAsync(expectedFile);

            // Act
            var result = await _fileService.GetFileById(fileId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileId, result.Id);
            Assert.Equal(expectedFile.FileName, result.FileName);
            Assert.Equal(expectedFile.FileSize, result.FileSize);
            Assert.Equal(expectedFile.Uploader, result.Uploader);
            Assert.Equal(expectedFile.UploadDate, result.UploadDate);
            Assert.Equal(expectedFile.FileContent, result.FileContent);
        }

        [Fact]
        public async Task GetFileById_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            int nonExistentId = 999;

            // Mocka direkt på _context.UploadedFile.FindAsync istället för DbSet
            _mockContext.Setup(c => c.UploadedFile.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UploadedFile?)null);

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _fileService.GetFileById(nonExistentId));
        }


        /// <summary>
        /// Helper method to create a mock IFormFile
        /// </summary>
        private Mock<IFormFile> CreateMockFormFile(byte[] content, string fileName, long length)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(length);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) =>
                {
                    return Task.Run(() =>
                    {
                        stream.Write(content, 0, content.Length);
                    }, token);
                });

            return mockFile;
        }
    }
}
