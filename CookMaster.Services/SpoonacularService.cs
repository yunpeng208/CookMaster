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
    /// <summary>
    /// Facade Pattern:
    /// - Provides a simple API (RecipesByIngredients, GetRecipeNutritions, GetRecipesInformation) to the rest of the application.
    /// - Hides the complexity of:
    ///   -- Calling the external Spoonacular API
    ///   -- Talking to the PostgreSQL storage
    ///   -- Caching logic
    /// Controllers only talk to this service, not directly to API and DB.
    /// </summary>
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

        // Facade + Cache-Aside:
        // 1. Try to get recipes from local storage (cache).
        // 2. If not enough results, call external Spoonacular API.
        // 3. Cache the API results in the database.
        // 4. Return a unified response to the caller.
        public async Task<ListResponse<RecipeSearchResult>> RecipesByIngredients(Dictionary<string, string> queryPrams)
        {
            try
            {
                if (queryPrams == null || !queryPrams.Any())
                    throw new ArgumentNullException(nameof(queryPrams));

                var ingredients = queryPrams.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)).Select(kv => kv.Value.Split(",")).SelectMany(x => x).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();
                var storage = storageFactory.GetStorage();

                using(var conn = storage.OpenConnection())
                {
                    var respies = await storage.GetRecipesByIngredients(conn, ingredients);

                    if (respies.Count >= 5)
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

        // Cache-Aside Pattern for nutrition:
        // 1) Try to load NutritionInfo from DB.
        // 2) If not found, call Spoonacular API.
        // 3) Save NutritionInfo into DB, then return it.
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

        // Cache-Aside Pattern for full recipe information:
        // 1) Try to load Recipe + ingredients from DB.
        // 2) If missing or incomplete, fetch from Spoonacular API.
        // 3) Save recipe data into DB, then return it.
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
