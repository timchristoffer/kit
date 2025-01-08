using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using KitBackend.Models;
using KitBackend.Models.Data;

namespace KitBackend.Services
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _context;

        public FileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UploadedFile?> UploadFileAsync(IFormFile file, string uploader)
        {
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("File is null or empty.");
                return null;
            }

            var allowedExtensions = new[] { ".cs", ".js", ".jsx", ".ts", ".tsx", ".css" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                Console.WriteLine($"Invalid file type: {fileExtension}. Only specific types are allowed.");
                return null;
            }

            var uploadedFile = new UploadedFile
            {
                FileName = file.FileName,
                FileSize = file.Length,
                Uploader = uploader,
                UploadDate = DateTime.UtcNow,
                FileContent = await ReadFileContentAsync(file),
            };

            _context.UploadedFile.Add(uploadedFile);
            await _context.SaveChangesAsync();

            Console.WriteLine($"File uploaded: Id={uploadedFile.Id}, Name={uploadedFile.FileName}, Size={uploadedFile.FileSize} bytes, Date={uploadedFile.UploadDate}");

            return uploadedFile; // Return the full UploadedFile object
        }

        private async Task<byte[]> ReadFileContentAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<UploadedFile> GetFileById(int id)
        {
            var file = await _context.UploadedFile.FindAsync(id);

            if (file == null)
            {
                Console.WriteLine($"File with Id={id} not found.");
                throw new FileNotFoundException($"File with Id={id} not found.");
            }

            Console.WriteLine($"Fetched File: Id={file.Id}, Name={file.FileName}, Size={file.FileSize} bytes, Date={file.UploadDate}");
            return file;
        }
    }
}
