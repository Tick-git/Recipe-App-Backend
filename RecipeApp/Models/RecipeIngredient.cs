﻿namespace RecipeApp.Models
{
    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        public required Recipe Recipe { get; set; }

        public int IngredientId { get; set; }
        public required Ingredient Ingredient { get; set; }

        public required double Quantity { get; set; }

        public required QuantityUnit QuantityUnit { get; set; }
    }
}
