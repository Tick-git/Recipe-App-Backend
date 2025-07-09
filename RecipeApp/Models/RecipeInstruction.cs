using RecipeApp.Models;

public class RecipeInstruction
{
    public int RecipeId { get; set; }
    public required Recipe Recipe { get; set;  }

    public int Step { get; set; }
    public string? Text { get; set; }
}