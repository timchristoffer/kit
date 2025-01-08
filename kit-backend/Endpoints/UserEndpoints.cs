using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using KitBackend.Models;
using KitBackend.Services;
using KitBackend.Models.Data;

namespace KitBackend.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapPost("/users", async (User user, IUserService userService) =>
            {
                if (user == null)
                {
                    return Results.BadRequest("User data is required.");
                }

                var createdUser = await userService.CreateUserAsync(user);
                return Results.Created($"/users/{createdUser.UserId}", createdUser);
            })
            .WithName("CreateUser")
            .Produces<User>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

            routes.MapGet("/users/{id}", async (int id, IUserService userService) =>
            {
                var user = await userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return Results.NotFound("User not found.");
                }

                return Results.Ok(user);
            })
            .WithName("GetUser")
            .Produces<User>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}

