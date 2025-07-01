using RecipeApp.Data;
using RecipeApp.Service;

namespace RecipeApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string recipeName = "Cookies";

            var webAppBuilder = WebApplication.CreateBuilder(args);
            webAppBuilder.Services.AddCors();

            var app = webAppBuilder.Build();

            app.UseCors(policy => policy.AllowAnyOrigin());

            app.MapGet("/", () =>
            {
                using var context = new RecipeContext();
                IRecipeService recipeService = new RecipeService(
                    new RecipeRepository(context),
                    new IngredientRepository(context),
                    new QuantityUnitRepository(context)
                );

                return recipeService.GetRecipeByName(recipeName);
            });

            app.Run();
        }
    }
}
