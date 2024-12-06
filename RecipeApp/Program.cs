using Microsoft.EntityFrameworkCore;

namespace RecipeApp
{
    public class Program
    {

        public static void Main(string[] args)
        {
            using (var context = new RecipeContext())
            {
                string recipeName = "Cookies";

                IRecipeService recipeService = new RecipeService(
                    new RecipeRepository(context), 
                    new IngredientRepository(context),
                    new QuantityUnitRepository(context));

                RecipeDto? cookieRecipeDto = recipeService.GetRecipeByName(recipeName);

                if (cookieRecipeDto == null)
                {
                    cookieRecipeDto = GetCookieRecipeDto(recipeName);

                    recipeService.AddRecipe(cookieRecipeDto);

                    Console.WriteLine("Cookie Recipe added");
                }
                else
                {
                    Console.WriteLine("Cookie Recipe needs: ");    
                    
                    foreach(var recipeIngredientDto in cookieRecipeDto.RecipeIngredientDtos)
                    {
                        Console.WriteLine($"    {recipeIngredientDto.Name}  {recipeIngredientDto.Quantity} {recipeIngredientDto.UnitSymbol}");
                    }
                }
            }
        }

        private static RecipeDto GetCookieRecipeDto(string recipeName)
        {
            List<RecipeIngredientDto> recipeIngredientDto = new()
            {
                new RecipeIngredientDto() { Name = "Sugar", Quantity = 100.0f, UnitSymbol = "g"},
                new RecipeIngredientDto() { Name = "Flour", Quantity = 200.0f, UnitSymbol = "g"},
                new RecipeIngredientDto() { Name = "Baking soda", Quantity = 1.0f, UnitSymbol = "tsp"},

            };

            return new RecipeDto() { Name = recipeName, RecipeIngredientDtos = recipeIngredientDto }; 
        }
    }

    public interface IRecipeRepository
    {
        public void AddRecipe(Recipe recipe);

        public Recipe? GetRecipeByName(string name);
    }

    public class RecipeRepository : IRecipeRepository
    {
        RecipeContext _recipeContext;

        public RecipeRepository(RecipeContext recipeContext)
        {
            _recipeContext = recipeContext;
        }

        public void AddRecipe(Recipe recipe)
        {
            _recipeContext.Recipes.Add(recipe);

            _recipeContext.SaveChanges();
        }

        public Recipe? GetRecipeByName(string name)
        {
            return _recipeContext.Recipes
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.QuantityUnit)
                    .Where(r => r.Name == name)
                    .FirstOrDefault();
        }
    }

    public interface IRecipeService
    {
        public void AddRecipe(RecipeDto recipe);
        public RecipeDto? GetRecipeByName(string name); 
    }

    public class RecipeDto
    {
        public required string Name { get; set; }
        public List<RecipeIngredientDto> RecipeIngredientDtos { get; set; } = new();
    }

    public class RecipeIngredientDto
    {
        public required string Name { get; set; }
        public required double Quantity { get; set; }

        public required string UnitSymbol { get; set; }
    }

    public class RecipeService : IRecipeService
    {
        IRecipeRepository _recipeRepository;
        IIngredientRepository _ingredientRepository;
        IQuantityUnitRepository _quantityRepository;

        public RecipeService(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository, IQuantityUnitRepository quantityUnitRepository)
        {
            _recipeRepository = recipeRepository;
            _ingredientRepository = ingredientRepository;
            _quantityRepository = quantityUnitRepository;
        }

        public void AddRecipe(RecipeDto recipeDto)
        {
            Recipe recipe = new() { Name = recipeDto.Name };

            foreach(var recipeIngredientDto in recipeDto.RecipeIngredientDtos)
            {
                Ingredient? ingredient = _ingredientRepository.GetIngredientByName(recipeIngredientDto.Name);
                QuantityUnit? quantityUnit = _quantityRepository.GetQuantityUnitBySymbol(recipeIngredientDto.UnitSymbol);

                if(ingredient == null)
                {
                    ingredient = new Ingredient() { Name = recipeIngredientDto.Name };
                }

                if(quantityUnit == null)
                {
                    quantityUnit = new QuantityUnit() { Name = "ToDO", Symbol = recipeIngredientDto.UnitSymbol };
                }

                RecipeIngredient recipeIngredient = new() 
                { 
                    Ingredient = ingredient, 
                    Recipe = recipe, 
                    Quantity = recipeIngredientDto.Quantity, 
                    QuantityUnit = quantityUnit 
                };

                recipe.RecipeIngredients.Add(recipeIngredient);
            }

            _recipeRepository.AddRecipe(recipe);
        }

        public RecipeDto? GetRecipeByName(string name)
        {
            Recipe? recipe = _recipeRepository.GetRecipeByName(name);

            if (recipe == null)
                return null;

            List<RecipeIngredientDto> recipeIngredientDtos = new();

            foreach(var recipeIngredient in recipe.RecipeIngredients)
            {
                recipeIngredientDtos.Add(new RecipeIngredientDto()
                {
                    Name = recipeIngredient.Ingredient.Name,
                    Quantity = recipeIngredient.Quantity,
                    UnitSymbol = recipeIngredient.QuantityUnit.Symbol
                });                   
            }

            return new RecipeDto() { Name = recipe.Name, RecipeIngredientDtos = recipeIngredientDtos};
        }
    }

    public interface IQuantityUnitRepository
    {
        public QuantityUnit? GetQuantityUnitBySymbol(string symbol);
    }

    public class QuantityUnitRepository : IQuantityUnitRepository
    {
        RecipeContext _recipeContext;

        public QuantityUnitRepository(RecipeContext recipeContext)
        {
            _recipeContext = recipeContext;
        }

        public QuantityUnit? GetQuantityUnitBySymbol(string symbol)
        {
            return _recipeContext.QuantityUnits
                .Where(qu => qu.Symbol == symbol)
                .FirstOrDefault();
        }
    }

    public interface IIngredientRepository
    {
        public Ingredient? GetIngredientByName(string name);
    }

    public class IngredientRepository : IIngredientRepository
    {
        RecipeContext _recipeContext;

        public IngredientRepository(RecipeContext recipeContext)
        {
            _recipeContext = recipeContext;
        }

        public Ingredient? GetIngredientByName(string name)
        {
            return _recipeContext.Ingredients
                .Where(i => i.Name == name)
                .FirstOrDefault();
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
