using RecipeApp.Models;

namespace RecipeApp.Data
{
    public interface IIngredientRepository
    {
        public Ingredient? GetIngredientByName(string name);
    }
}
