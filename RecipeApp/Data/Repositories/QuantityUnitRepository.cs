using RecipeApp.Models;

namespace RecipeApp.Data
{
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
}
