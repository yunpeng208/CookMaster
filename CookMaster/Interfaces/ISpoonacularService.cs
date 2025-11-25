using CookMaster.Models;
using CookMaster.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Interfaces
{
    public interface ISpoonacularService
    {
        Task<ListResponse<RecipeSearchResult>> RecipesByIngredients(Dictionary<string, string> queryPrams);
        Task<SingletonResponse<NutritionInfo>> GetRecipeNutritions(int recipeID);
        Task<SingletonResponse<Recipe>> GetRecipesnformation(int recipeID);
    }
}
