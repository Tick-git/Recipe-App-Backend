namespace RecipeApp.Dto
{
    public class RecipeIngredientDto
    {
        public required string Name { get; set; }
        public required double Quantity { get; set; }

        public required string UnitSymbol { get; set; }
    }
}
