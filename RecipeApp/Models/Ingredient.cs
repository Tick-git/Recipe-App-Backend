namespace RecipeApp.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public List<RecipeIngredient> RecipeIngredients = new();
    }
}
