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
        Task<ListResponse<Recipe>> RecipesByIngredients(Dictionary<string, string> queryPrams);
        Task<ListResponse<NutritionInfo>> GetRecipesNutritions(long recipeID);
    }
}
