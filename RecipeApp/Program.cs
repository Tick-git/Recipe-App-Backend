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

            var app = webAppBuilder.Build();

            app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader());

            app.MapGet("/recipes/{recipeName}", (string recipeName) =>
            {
                using var context = new RecipeContext();
                IRecipeService recipeService = new RecipeService(
                    new RecipeRepository(context),
                    new IngredientRepository(context),
                    new QuantityUnitRepository(context)
                );

                return recipeService.GetRecipeByName(recipeName);
            });

            app.MapPost("/recipes", (RecipeDto dto) =>
            {
                using var context = new RecipeContext();
                var recipeService = new RecipeService(
                    new RecipeRepository(context),
                    new IngredientRepository(context),
                    new QuantityUnitRepository(context)
                );
                recipeService.AddRecipe(dto);
                return Results.Created($"/recipes/{dto.GeneralData.Name}", dto);
            });

            app.Run();
        }
    }
}
