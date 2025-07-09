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

            AddRecipeIngredients(recipeDto, recipe);

            AddRecipeInstructions(recipeDto, recipe);

            _recipeRepository.AddRecipe(recipe);
        }

        private void AddRecipeInstructions(RecipeDto recipeDto, Recipe recipe)
        {
            foreach(var instructionDto in recipeDto.Instructions)
            {
                RecipeInstruction recipeInstruction = new()
                {
                    Recipe = recipe,
                    Step = instructionDto.Step,
                    Text = instructionDto.Text
                };

                recipe.RecipeInstructions.Add(recipeInstruction);
            }
        }

        private void AddRecipeIngredients(RecipeDto recipeDto, Recipe recipe)
        {
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
        }

        public RecipeDto? GetRecipeByName(string name)
        {
            Recipe? recipe = _recipeRepository.GetRecipeByName(name);

            if (recipe == null)
                return null;
            
            // TODO: #001
            GeneralDataDto generalData = new() { Name = recipe.Name, Author = "", Difficulty = "", Time = ""};
            List<InstructionDto> recipeInstructionsDto = new();
            List<IngredientDto> recipeIngredientsDtos = new();

            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                recipeIngredientsDtos.Add(new()
                {
                    Name = recipeIngredient.Ingredient.Name,
                    Quantity = recipeIngredient.Quantity.ToString(),
                    Unit = recipeIngredient.QuantityUnit.Symbol
                });
            }

            foreach(var recipeInstruction in recipe.RecipeInstructions)
            {
                recipeInstructionsDto.Add(new()
                {
                    Step = recipeInstruction.Step,
                    Text = recipeInstruction.Text
                });
            }

            return new RecipeDto() { GeneralData = generalData, Ingredients = recipeIngredientsDtos, Instructions = recipeInstructionsDto };
        }
    }
}

// Issue: #001
// Extend the Recipe Model to include classes GeneralData
