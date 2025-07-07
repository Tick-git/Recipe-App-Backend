using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Dto;
using RecipeApp.Service;

namespace RecipeApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webAppBuilder = WebApplication.CreateBuilder(args);
            webAppBuilder.Services.AddCors();

            webAppBuilder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
            webAppBuilder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
            webAppBuilder.Services.AddScoped<IQuantityUnitRepository, QuantityUnitRepository>();
            webAppBuilder.Services.AddScoped<IRecipeService, RecipeService>();

            webAppBuilder.Services.AddDbContext<RecipeContext>(opts => 
                opts.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;")
            );

            var app = webAppBuilder.Build();

            app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader());

            app.MapGet("/recipes/{recipeName}", (string recipeName, IRecipeService recipeService) =>
            {
                return recipeService.GetRecipeByName(recipeName);
            });

            app.MapPost("/recipes", (RecipeDto dto, IRecipeService recipeService) =>
            {
                recipeService.AddRecipe(dto);

                return Results.Created($"/recipes/{dto.GeneralData.Name}", dto);
            });

            app.Run();
        }
    }
}
