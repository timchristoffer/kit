using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using KitBackend.Models.Data;
using KitBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KitBackend.Endpoints
{
    public static class CodeSnippetEndpoints
    {
        public static void MapCodeSnippetEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapPost("/api/codesnippets", CreateCodeSnippet)
                .WithName("CreateCodeSnippet")
                .Produces<CodeSnippet>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithTags("CodeSnippets");

            routes.MapGet("/api/codesnippets/{id}", GetCodeSnippetById)
                .WithName("GetCodeSnippet")
                .Produces<CodeSnippet>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithTags("CodeSnippets");
        }

        public static async Task<IResult> CreateCodeSnippet(CodeSnippet snippet, ICodeSnippetService snippetService)
        {
            try
            {
                if (snippet == null)
                {
                    Console.WriteLine("Snippet data is required.");
                    return Results.BadRequest("Snippet data is required.");
                }

                var createdSnippet = await snippetService.CreateCodeSnippetAsync(snippet);
                Console.WriteLine($"Snippet created with ID: {createdSnippet.SnippetId}");
                return Results.Created($"/api/codesnippets/{createdSnippet.SnippetId}", createdSnippet);
            }
            catch (ArgumentException argEx)
            {
                Console.WriteLine($"Argument exception: {argEx.Message}");
                return Results.BadRequest($"Invalid input: {argEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
                return Results.Problem(
                    detail: "An unexpected error occurred while creating the snippet.",
                    statusCode: 500
                );
            }
        }

        public static async Task<IResult> GetCodeSnippetById(Guid id, ICodeSnippetService snippetService)
        {
            try
            {
                Console.WriteLine($"Fetching snippet with ID: {id}");

                var snippet = await snippetService.GetCodeSnippetByIdAsync(id);
                if (snippet != null)
                {
                    Console.WriteLine($"Snippet found: {snippet.SnippetId}");
                    return Results.Ok(snippet);
                }

                Console.WriteLine("Snippet not found.");
                return Results.NotFound("Snippet not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while fetching snippet: {ex.Message}");
                return Results.Problem(
                    detail: "An unexpected error occurred while fetching the snippet.",
                    statusCode: 500
                );
            }
        }
    }
}
