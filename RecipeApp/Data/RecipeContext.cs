using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;

namespace RecipeApp.Data
{
    public class RecipeContext : DbContext
    {
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<QuantityUnit> QuantityUnits { get; set; }
        public DbSet<RecipeInstruction> RecipeInstructions { get; set; }

        public RecipeContext(DbContextOptions<RecipeContext> options) : base(options)
        {
            Database.EnsureCreated();
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

            modelBuilder.Entity<RecipeInstruction>()
                .HasKey(ri => new { ri.RecipeId, ri.Step });

            modelBuilder.Entity<RecipeInstruction>()
                .HasOne(ri => ri.Recipe)
                .WithMany(ri => ri.RecipeInstructions)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<QuantityUnit>().HasData(
                new QuantityUnit { Id = 1, Name = "Gram", Symbol = "g" },
                new QuantityUnit { Id = 2, Name = "Kilogram", Symbol = "kg" },
                new QuantityUnit { Id = 3, Name = "Liter", Symbol = "l" },
                new QuantityUnit { Id = 4, Name = "Milliliter", Symbol = "ml" },
                new QuantityUnit { Id = 5, Name = "Teaspoon", Symbol = "tsp" },
                new QuantityUnit { Id = 6, Name = "Tablespoon", Symbol = "tbsp" });
        }
    }
}
