using CookMaster.Models;
using CookMaster.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Interfaces
{
    public interface ISpoonacularClient
    {
        Task<ListResponse<Recipe>> SearchRecipesByIngredients(Dictionary<string, string> queryPrams);
        Task<ListResponse<NutritionInfo>> NutritionsByID(long recipeID);
    }
}
