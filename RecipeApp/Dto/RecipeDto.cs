namespace RecipeApp.Dto
{
    public class RecipeDto
    {
        public required string Name { get; set; }
        public List<RecipeIngredientDto> RecipeIngredientDtos { get; set; } = new();
    }
}
