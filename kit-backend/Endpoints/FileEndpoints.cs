using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using KitBackend.Models;
using KitBackend.Services;
using KitBackend.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KitBackend.Endpoints
{
    public static class FileEndpoints
    {
        public static void MapFileEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/files", UploadFile)
               .WithName("UploadFile")
               .Produces<IResult>()
               .WithTags("Files")
               .DisableAntiforgery();

            app.MapGet("/api/files/{id}", GetFileById)
               .WithName("GetFileById")
               .Produces<IResult>()
               .WithTags("Files");
        }

        public static async Task<IResult> UploadFile(IFormFile file, IFileService fileService)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    Console.WriteLine("File is null or empty.");
                    return Results.BadRequest("File is required. Please upload a valid file.");
                }

                var allowedExtensions = new[] { ".cs", ".js", ".jsx", ".ts", ".tsx", ".py" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Results.BadRequest("Invalid file type. Allowed types are .cs, .js, .jsx, .ts, .tsx, .py.");
                }

                // Assume you get "uploader" from authentication (e.g., User.Identity.Name)
                string uploader = "uploader"; // Replace with actual source if possible.

                var uploadedFile = await fileService.UploadFileAsync(file, uploader);
                if (uploadedFile == null)
                {
                    Console.WriteLine("File upload failed in the service layer.");
                    return Results.BadRequest("File upload failed due to a service error.");
                }

                Console.WriteLine($"File uploaded successfully: {uploadedFile.FileName}");
                return Results.Ok(new
                {
                    Id = uploadedFile.Id,
                    FileName = uploadedFile.FileName,
                    FileSize = uploadedFile.FileSize,
                    UploadDate = uploadedFile.UploadDate,
                    Uploader = uploadedFile.Uploader
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
                return Results.Problem(
                    detail: "An unexpected error occurred while uploading the file.",
                    statusCode: 500
                );
            }
        }

        public static async Task<IResult> GetFileById(int id, IFileService fileService)
        {
            try
            {
                Console.WriteLine($"Fetching file with ID: {id}");

                var file = await fileService.GetFileById(id);
                if (file != null)
                {
                    Console.WriteLine($"File found: {file.FileName}, Size: {file.FileContent.Length} bytes");

                    var mimeType = GetMimeType(file.FileName);
                    return Results.File(file.FileContent, mimeType, file.FileName);
                }

                Console.WriteLine("File not found.");
                return Results.NotFound(new { Message = "File not found." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while fetching file: {ex.Message}");
                return Results.Problem(
                    detail: "An unexpected error occurred while fetching the file.",
                    statusCode: 500
                );
            }
        }

        private static string GetMimeType(string fileName)
        {
            return fileName.EndsWith(".cs") ? "text/plain" :
                   fileName.EndsWith(".js") ? "application/javascript" :
                   fileName.EndsWith(".jsx") ? "application/javascript" :
                   fileName.EndsWith(".ts") ? "application/typescript" :
                   fileName.EndsWith(".tsx") ? "application/typescript" :
                   fileName.EndsWith(".py") ? "text/x-python" :
                   "application/octet-stream";
        }
    }
}
