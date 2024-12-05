using Microsoft.EntityFrameworkCore;

namespace RecipeApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var context = new RecipeContext())
            {
                Recipe? cookieRecipe = context.Recipes
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.QuantityUnit)
                    .Where(r => r.Name == "Cookies")
                    .FirstOrDefault();

                if (cookieRecipe == null)
                {
                    AddCookieRecipe(context);
                    Console.WriteLine("Cookie Recipe added");
                } 
                else
                {
                    Console.WriteLine("Cookie Recipe needs: ");

                    foreach(var recipeIngredient in cookieRecipe.RecipeIngredients)
                    {
                        Console.WriteLine($"    {recipeIngredient.Quantity} {recipeIngredient.QuantityUnit.Symbol}  {recipeIngredient.Ingredient.Name}");
                    }
                }
            }
        }

        private static void AddCookieRecipe(RecipeContext context)
        {
            Recipe cookieRecipe = new() { Name = "Cookies" };

            Ingredient sugar = new() { Name = "Sugar" };
            Ingredient flour = new() { Name = "Flour" };
            Ingredient bakingsoda = new() { Name = "Baking soda" };

            QuantityUnit gramm = new() { Name = "Gramm", Symbol = "g"};
            QuantityUnit teaspoon = new() { Name = "Teaspoon", Symbol = "tsp"};

            List<RecipeIngredient> cookieRecipeIngredients = new()
            {
                { new() { Recipe = cookieRecipe, Ingredient = sugar, Quantity = 100.0f, QuantityUnit = gramm }},
                { new() { Recipe = cookieRecipe, Ingredient = flour, Quantity = 200.0f, QuantityUnit = gramm }},
                { new() { Recipe = cookieRecipe, Ingredient = bakingsoda, Quantity = 1.0f, QuantityUnit = teaspoon}}
            };

            cookieRecipe.RecipeIngredients.AddRange(cookieRecipeIngredients);

            context.Recipes.Add(cookieRecipe);

            context.SaveChanges();
        }
    }

    public class RecipeContext : DbContext
    {
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<QuantityUnit> QuantityUnits { get; set; }

        public RecipeContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId);
        }
    }

    public class Ingredient
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public List<RecipeIngredient> RecipeIngredients = new();
    }

    public class Recipe
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
    }

    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        public required Recipe Recipe { get; set; }

        public int IngredientId { get; set; }
        public required Ingredient Ingredient { get; set; }

        public required double Quantity { get; set; }

        public required QuantityUnit QuantityUnit { get; set; }
    }

    public class QuantityUnit
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Symbol { get; set; }
    }
}
