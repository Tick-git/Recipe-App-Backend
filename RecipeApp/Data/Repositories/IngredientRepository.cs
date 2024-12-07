using RecipeApp.Models;

namespace RecipeApp.Data
{
    public class IngredientRepository : IIngredientRepository
    {
        RecipeContext _recipeContext;

        public IngredientRepository(RecipeContext recipeContext)
        {
            _recipeContext = recipeContext;
        }

        public Ingredient? GetIngredientByName(string name)
        {
            return _recipeContext.Ingredients
                .Where(i => i.Name == name)
                .FirstOrDefault();
        }
    }
}
