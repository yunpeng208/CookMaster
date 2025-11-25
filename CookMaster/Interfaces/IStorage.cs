using CookMaster.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Interfaces
{
    public interface IStorage
    {
        #region CTX
        IDbConnection OpenConnection(TimeSpan? timeout = null);
        #endregion


        Task<bool> DatabaseExistsAsync(IDbConnection conn);

        #region Recipe
        Task CacheRecipes(IDbConnection conn, List<RecipeSearchResult> results);
        Task SaveRecipe(IDbConnection conn, Recipe recipe);
        Task<Recipe> GetRecipe(IDbConnection conn, int recipeID, bool full = false);
        Task UpdateRecipe(IDbConnection conn, Recipe recipe);
        Task<List<RecipeSearchResult>> GetRecipesByIngredients(IDbConnection conn, List<string> ingredientNames);
        #endregion

        #region RecipesNutritions
        Task SaveRecipeNutritions(IDbConnection conn, NutritionInfo nutritionInfo);
        Task<NutritionInfo> GetRecipeNutritions(IDbConnection conn, int recipeID);
        #endregion

    }
}
