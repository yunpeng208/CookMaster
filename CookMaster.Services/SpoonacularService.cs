using CookMaster.Interfaces;
using CookMaster.Models;
using CookMaster.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Services
{
    public class SpoonacularService: ISpoonacularService
    {
        private readonly ILogger logger;
        private readonly ISpoonacularClientFactory spoonacularClientFactory;
        public SpoonacularService(ILoggerFactory loggerFactory, ISpoonacularClientFactory spoonacularClientFactory)
        {
            this.logger = loggerFactory.CreateLogger<SpoonacularService>();
            this.spoonacularClientFactory = spoonacularClientFactory;
        }

        public async Task<ListResponse<Recipe>> RecipesByIngredients(Dictionary<string, string> queryPrams)
        {
            try
            {
                if (queryPrams == null || !queryPrams.Any())
                    throw new ArgumentNullException(nameof(queryPrams));


                var client = spoonacularClientFactory.GetClient();

                var response = await client.SearchRecipesByIngredients(queryPrams);

                if (!response.Success)
                    throw new Exception(response.Message);


                return response;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during SearchRecipesByIngredients");
                return new ListResponse<Recipe>()
                {
                    Message = ex.Message,
                };
            }
        }

        public async Task<ListResponse<NutritionInfo>> GetRecipesNutritions(long recipeID)
        {
            try
            {

                var client = spoonacularClientFactory.GetClient();

                var response = await client.NutritionsByID(recipeID);

                if (!response.Success)
                    throw new Exception(response.Message);


                return response;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during NutritionsByID");
                return new ListResponse<NutritionInfo>()
                {
                    Message = ex.Message,
                };
            }
        }
    }
}
