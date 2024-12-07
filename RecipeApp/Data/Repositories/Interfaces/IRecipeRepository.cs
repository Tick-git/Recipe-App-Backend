using RecipeApp.Models;

namespace RecipeApp.Data
{
    public interface IRecipeRepository
    {
        public void AddRecipe(Recipe recipe);

        public Recipe? GetRecipeByName(string name);
    }
}
