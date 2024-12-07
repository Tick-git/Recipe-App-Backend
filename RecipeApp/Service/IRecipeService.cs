using RecipeApp.Dto;

namespace RecipeApp.Service
{
    public interface IRecipeService
    {
        public void AddRecipe(RecipeDto recipe);
        public RecipeDto? GetRecipeByName(string name);
    }
}
