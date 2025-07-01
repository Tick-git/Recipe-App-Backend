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
            // TODO: #001
            Recipe recipe = new() { Name = recipeDto.GeneralData.Name };

            foreach (var recipeIngredientDto in recipeDto.Ingredients)
            {
                Ingredient? ingredient = _ingredientRepository.GetIngredientByName(recipeIngredientDto.Name);
                QuantityUnit? quantityUnit = _quantityRepository.GetQuantityUnitBySymbol(recipeIngredientDto.Unit);

                if (ingredient == null)
                {
                    ingredient = new Ingredient() { Name = recipeIngredientDto.Name };
                }

                // TODO: Validation in ServiceLayer?
                // https://stackoverflow.com/questions/16793982/separating-the-service-layer-from-the-validation-layer

                if (quantityUnit == null)
                {
                    throw new InvalidOperationException($"Unit symbol: {recipeIngredientDto.Unit} could not be found in database");
                }

                RecipeIngredient recipeIngredient = new()
                {
                    Ingredient = ingredient,
                    Recipe = recipe,
                    Quantity = double.Parse(recipeIngredientDto.Quantity),
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
            
            // TODO: #001
            GeneralDataDto generalData = new() { Name = recipe.Name, Author = "", Difficulty = "", Time = ""};
            List<InstructionDto> instructions = new();
            List<IngredientDto> recipeIngredientDtos = new();

            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                recipeIngredientDtos.Add(new IngredientDto()
                {
                    Name = recipeIngredient.Ingredient.Name,
                    Quantity = recipeIngredient.Quantity.ToString(),
                    Unit = recipeIngredient.QuantityUnit.Symbol
                });
            }

            return new RecipeDto() { GeneralData = generalData, Ingredients = recipeIngredientDtos, Instructions = instructions };
        }
    }
}

// Issue: #001
// Extend the Recipe Model to include classes GeneralData + Instructions
