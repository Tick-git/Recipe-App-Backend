using Microsoft.EntityFrameworkCore;

namespace RecipeApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var context = new IngredientContext())
            {
                context.Ingredients.Add(new Ingredient { Name = "Salt" });
                context.SaveChanges();

                var ingredients = context.Ingredients.ToList();

                foreach (var ingredient in ingredients)
                {
                    Console.WriteLine($"Name: {ingredient.Name}");
                }
            }
        }
    }

    public class Ingredient
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class IngredientContext : DbContext
    {
        public DbSet<Ingredient> Ingredients { get; set; }

        public IngredientContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
