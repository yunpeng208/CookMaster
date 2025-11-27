using CookMaster.Interfaces;
using CookMaster.Models;
using CookMaster.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookMaster.Services
{
    public class SpoonacularService: ISpoonacularService
    {
        private readonly ILogger logger;
        private readonly ISpoonacularClientFactory spoonacularClientFactory;
        private readonly IStorageFactory storageFactory;
        public SpoonacularService(ILoggerFactory loggerFactory, ISpoonacularClientFactory spoonacularClientFactory, IStorageFactory storageFactory)
        {
            this.logger = loggerFactory.CreateLogger<SpoonacularService>();
            this.spoonacularClientFactory = spoonacularClientFactory;
            this.storageFactory = storageFactory;
        }

        public async Task<ListResponse<RecipeSearchResult>> RecipesByIngredients(Dictionary<string, string> queryPrams)
        {
            try
            {
                if (queryPrams == null || !queryPrams.Any())
                    throw new ArgumentNullException(nameof(queryPrams));

                var ingredients = queryPrams.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)).Select(kv => kv.Value.Split(",")).SelectMany(x => x).ToList();
                var storage = storageFactory.GetStorage();

                using(var conn = storage.OpenConnection())
                {
                    var respies = await storage.GetRecipesByIngredients(conn, ingredients);

                    if (respies.Count >= 2)
                    {
                        return new ListResponse<RecipeSearchResult>()
                        {
                            Success = true,
                            Objects = respies
                        };
                    }
                }

                var client = spoonacularClientFactory.GetClient();
                var response = await client.SearchRecipesByIngredients(queryPrams);

                if (!response.Success)
                    throw new Exception(response.Message);

                using (var conn = storage.OpenConnection())
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        await storage.CacheRecipes(conn, response.Objects);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error during cachin Recipes");
                        transaction.Rollback();
                    }

                }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during SearchRecipesByIngredients");
                return new ListResponse<RecipeSearchResult>()
                {
                    Message = ex.Message,
                };
            }
        }

        public async Task<SingletonResponse<NutritionInfo>> GetRecipeNutritions(int recipeID)
        {
            try
            {
                var storage = storageFactory.GetStorage();

                using (var conn = storage.OpenConnection())
                {
                    var nutritionInfo = await storage.GetRecipeNutritions(conn, recipeID);
                    if (nutritionInfo != null)
                        return new SingletonResponse<NutritionInfo>()
                        {
                            Success = true,
                            Object = nutritionInfo
                        };
                }

                var client = spoonacularClientFactory.GetClient();
                var response = await client.GetRecipeNutritions(recipeID);

                if (!response.Success)
                    throw new Exception(response.Message);

                using (var conn = storage.OpenConnection())
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        await storage.SaveRecipeNutritions(conn, response.Object);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error during saving Recipe {recipeID} NutritionInfo");
                        transaction.Rollback();
                    }

                }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Get Recipe NutritionInfo");
                return new SingletonResponse<NutritionInfo>()
                {
                    Message = ex.Message,
                };
            }
        }

        public async Task<SingletonResponse<Recipe>> GetRecipeInformation(int recipeID)
        {
            try
            {
                var storage = storageFactory.GetStorage();

                using (var conn = storage.OpenConnection())
                {
                    var recipe = await storage.GetRecipe(conn, recipeID, full: true);
                    if (recipe != null && recipe.ExtendedIngredients.Any())
                        return new SingletonResponse<Recipe>()
                        {
                            Success = true,
                            Object = recipe
                        };
                }

                var client = spoonacularClientFactory.GetClient();
                var response = await client.GetRecipeInformation(recipeID);

                if (!response.Success)
                    throw new Exception(response.Message);


                using (var conn = storage.OpenConnection())
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        await storage.SaveRecipe(conn, response.Object);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error during saving Recipe {recipeID}");
                        transaction.Rollback();
                    }

                }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during GetRecipeInformation");
                return new SingletonResponse<Recipe>()
                {
                    Message = ex.Message,
                };
            }
        }
    }
}
