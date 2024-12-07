namespace RecipeApp.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
    }
}
