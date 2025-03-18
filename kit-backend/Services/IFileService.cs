using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using KitBackend.Models;
using KitBackend.Models.Requests;
using KitBackend.Models.Responses;
using KitBackend.Models.Data;

namespace KitBackend.Services
{
    public interface IFileService
    {
        Task<UploadedFile?> UploadFileAsync(IFormFile file, string uploader);
        Task<UploadedFile> GetFileById(int id);
    }
}
