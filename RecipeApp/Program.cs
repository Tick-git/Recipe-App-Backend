using RecipeApp.Data;
using RecipeApp.Dto;
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

                RecipeDto? cookieRecipeDto = recipeService.GetRecipeByName(recipeName);

                if (cookieRecipeDto == null)
                {
                    cookieRecipeDto = GetCookieRecipeDto(recipeName);

                    recipeService.AddRecipe(cookieRecipeDto);

                    Console.WriteLine("Cookie Recipe added");
                }
                else
                {
                    Console.WriteLine("Cookie Recipe needs: ");    
                    
                    foreach(var recipeIngredientDto in cookieRecipeDto.RecipeIngredientDtos)
                    {
                        Console.WriteLine($"    {recipeIngredientDto.Name}  {recipeIngredientDto.Quantity} {recipeIngredientDto.UnitSymbol}");
                    }
                }
            }
        }

        private static RecipeDto GetCookieRecipeDto(string recipeName)
        {
            List<RecipeIngredientDto> recipeIngredientDto = new()
            {
                new RecipeIngredientDto() { Name = "Sugar", Quantity = 100.0f, UnitSymbol = "g"},
                new RecipeIngredientDto() { Name = "Flour", Quantity = 200.0f, UnitSymbol = "g"},
                new RecipeIngredientDto() { Name = "Baking soda", Quantity = 1.0f, UnitSymbol = "tsp"},

            };

            return new RecipeDto() { Name = recipeName, RecipeIngredientDtos = recipeIngredientDto }; 
        }
    }
}
