using RecipeApp.Models;

namespace RecipeApp.Data
{
    public interface IQuantityUnitRepository
    {
        public QuantityUnit? GetQuantityUnitBySymbol(string symbol);
    }
}
