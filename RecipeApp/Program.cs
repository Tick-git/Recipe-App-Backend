using RecipeApp.Data;
using RecipeApp.Service;

namespace RecipeApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var context = new RecipeContext())
            {
                string recipeName = "Cookies";

                IRecipeService recipeService = new RecipeService(
                    new RecipeRepository(context),
                    new IngredientRepository(context),
                    new QuantityUnitRepository(context));

                var builder = WebApplication.CreateBuilder(args);
                var app = builder.Build();

                app.MapGet("/", () => recipeService.GetRecipeByName(recipeName));

                app.Run();
            }
        }
    }
}
