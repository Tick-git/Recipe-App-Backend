using RecipeApp.Data;
using RecipeApp.Dto;
using RecipeApp.Models;

namespace RecipeApp.Service
{
    public class RecipeService : IRecipeService
    {
        IRecipeRepository _recipeRepository;
        IIngredientRepository _ingredientRepository;
        IQuantityUnitRepository _quantityRepository;

        public RecipeService(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository, IQuantityUnitRepository quantityUnitRepository)
        {
            _recipeRepository = recipeRepository;
            _ingredientRepository = ingredientRepository;
            _quantityRepository = quantityUnitRepository;
        }

        public void AddRecipe(RecipeDto recipeDto)
        {
            Recipe recipe = new() { Name = recipeDto.Name };

            foreach (var recipeIngredientDto in recipeDto.RecipeIngredientDtos)
            {
                Ingredient? ingredient = _ingredientRepository.GetIngredientByName(recipeIngredientDto.Name);
                QuantityUnit? quantityUnit = _quantityRepository.GetQuantityUnitBySymbol(recipeIngredientDto.UnitSymbol);

                if (ingredient == null)
                {
                    ingredient = new Ingredient() { Name = recipeIngredientDto.Name };
                }

                if(quantityUnit == null)
                {
                    quantityUnit = new QuantityUnit() { Name = "TODO", Symbol = recipeIngredientDto.UnitSymbol };
                }

                RecipeIngredient recipeIngredient = new()
                {
                    Ingredient = ingredient,
                    Recipe = recipe,
                    Quantity = recipeIngredientDto.Quantity,
                    QuantityUnit = quantityUnit
                };

                recipe.RecipeIngredients.Add(recipeIngredient);
            }

            _recipeRepository.AddRecipe(recipe);
        }

        public RecipeDto? GetRecipeByName(string name)
        {
            Recipe? recipe = _recipeRepository.GetRecipeByName(name);

            if (recipe == null)
                return null;

            List<RecipeIngredientDto> recipeIngredientDtos = new();

            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                recipeIngredientDtos.Add(new RecipeIngredientDto()
                {
                    Name = recipeIngredient.Ingredient.Name,
                    Quantity = recipeIngredient.Quantity,
                    UnitSymbol = recipeIngredient.QuantityUnit.Symbol
                });
            }

            return new RecipeDto() { Name = recipe.Name, RecipeIngredientDtos = recipeIngredientDtos };
        }
    }
}
