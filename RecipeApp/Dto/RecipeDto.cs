

namespace RecipeApp.Dto
{
    public class RecipeDto
    {
        public required GeneralDataDto GeneralData { get; set; }
        public List<IngredientDto> Ingredients { get; set; } = new();
        public List<InstructionDto> Instructions { get; set; } = new();
    }
}