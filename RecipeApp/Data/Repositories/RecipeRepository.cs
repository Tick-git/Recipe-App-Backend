using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;

namespace RecipeApp.Data
{
    public class RecipeRepository : IRecipeRepository
    {
        RecipeContext _recipeContext;

        public RecipeRepository(RecipeContext recipeContext)
        {
            _recipeContext = recipeContext;
        }

        public void AddRecipe(Recipe recipe)
        {
            _recipeContext.Recipes.Add(recipe);

            _recipeContext.SaveChanges();
        }

        public Recipe? GetRecipeByName(string name)
        {
            return _recipeContext.Recipes
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.QuantityUnit)
                    .Include(r => r.RecipeInstructions)
                    .Where(r => r.Name == name)
                    .FirstOrDefault();
        }
    }
}
