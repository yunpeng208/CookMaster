using CookMaster.Interfaces;
using CookMaster.Models;
using CookMaster.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public async Task<ListResponse<Recipe>> RecipesByIngredients(Dictionary<string, string> queryPrams)
        {
            try
            {
                if (queryPrams == null || !queryPrams.Any())
                    throw new ArgumentNullException(nameof(queryPrams));


                var client = spoonacularClientFactory.GetClient();

#if DEBUG
                // Only in debug mode. Since we have limited number of API calls we can make to Spoonacular
                // TODO: Will remove this after I get the insert and loading from DB working
                var r = @"
{
    ""Objects"": [
        {
            ""ID"": 640352,
            ""Image"": ""https://img.spoonacular.com/recipes/640352-312x231.jpg"",
            ""ImageType"": ""jpg"",
            ""Likes"": 11,
            ""MissedIngredientCount"": 3,
            ""MissedIngredients"": [
                {
                    ""Aisle"": ""Produce"",
                    ""Amount"": 2,
                    ""ExtendedName"": ""fresh cranberries"",
                    ""Id"": 9078,
                    ""Image"": ""https://img.spoonacular.com/ingredients_100x100/cranberries.jpg"",
                    ""Meta"": [
                        ""fresh""
                    ],
                    ""Name"": ""cranberries"",
                    ""Original"": ""2 cups fresh cranberries"",
                    ""OriginalName"": ""fresh cranberries"",
                    ""Unit"": ""cups"",
                    ""UnitLong"": ""cups"",
                    ""UnitShort"": ""cup""
                },
                {
                    ""Aisle"": ""Milk, Eggs, Other Dairy"",
                    ""Amount"": 4,
                    ""ExtendedName"": ""unsalted butter"",
                    ""Id"": 1145,
                    ""Image"": ""https://img.spoonacular.com/ingredients_100x100/butter-sliced.jpg"",
                    ""Meta"": [
                        ""unsalted"",
                        ""cut into cubes""
                    ],
                    ""Name"": ""butter"",
                    ""Original"": ""1/2 stick (4 Tbs) unsalted butter, cut into cubes"",
                    ""OriginalName"": ""1/2 stick unsalted butter, cut into cubes"",
                    ""Unit"": ""Tbs"",
                    ""UnitLong"": ""Tbs"",
                    ""UnitShort"": ""Tbsp""
                },
                {
                    ""Aisle"": ""Cereal"",
                    ""Amount"": 1.5,
                    ""Id"": 8120,
                    ""Image"": ""https://img.spoonacular.com/ingredients_100x100/rolled-oats.jpg"",
                    ""Meta"": [
                        ""(not quick-cooking)""
                    ],
                    ""Name"": ""regular oats"",
                    ""Original"": ""1 1/2 cups regular oats (not quick-cooking)"",
                    ""OriginalName"": ""regular oats (not quick-cooking)"",
                    ""Unit"": ""cups"",
                    ""UnitLong"": ""cups"",
                    ""UnitShort"": ""cup""
                }
            ],
            ""Title"": ""Cranberry Apple Crisp"",
            ""UnusedIngredients"": [],
            ""UsedIngredientCount"": 1,
            ""UsedIngredients"": [
                {
                    ""Aisle"": ""Produce"",
                    ""Amount"": 4,
                    ""Id"": 1089003,
                    ""Image"": ""https://img.spoonacular.com/ingredients_100x100/grannysmith-apple.png"",
                    ""Meta"": [
                        ""chopped""
                    ],
                    ""Name"": ""granny smith apples"",
                    ""Original"": ""4 cups Granny Smith apples, chopped into ½ inch chunks"",
                    ""OriginalName"": ""Granny Smith apples, chopped into ½ inch chunks"",
                    ""Unit"": ""cups"",
                    ""UnitLong"": ""cups"",
                    ""UnitShort"": ""cup""
                }
            ]
        },
        {
            ""ID"": 641803,
            ""Image"": ""https://img.spoonacular.com/recipes/641803-312x231.jpg"",
            ""ImageType"": ""jpg"",
            ""Likes"": 1,
            ""MissedIngredientCount"": 3,
            ""MissedIngredients"": [
                {
                    ""Aisle"": ""Milk, Eggs, Other Dairy"",
                    ""Amount"": 0.75,
                    ""Id"": 1001,
                    ""Image"": ""https://img.spoonacular.com/ingredients_100x100/butter-sliced.jpg"",
                    ""Meta"": [],
                    ""Name"": ""butter"",
                    ""Original"": ""3/4 stick of butter"",
                    ""OriginalName"": ""butter"",
                    ""Unit"": ""stick"",
                    ""UnitLong"": ""sticks"",
                    ""UnitShort"": ""stick""
                },
                {
                    ""Aisle"": ""Spices and Seasonings"",
                    ""Amount"": 1,
                    ""Id"": 2011,
                    ""Image"": ""https://img.spoonacular.com/ingredients_100x100/cloves.jpg"",
                    ""Meta"": [],
                    ""Name"": ""ground cloves"",
                    ""Original"": ""Dash of ground cloves"",
                    ""OriginalName"": ""ground cloves"",
                    ""Unit"": ""Dash"",
                    ""UnitLong"": ""Dash"",
                    ""UnitShort"": ""Dash""
                },
                {
                    ""Aisle"": ""Produce"",
                    ""Amount"": 1,
                    ""Id"": 9156,
                    ""Image"": ""https://img.spoonacular.com/ingredients_100x100/zest-lemon.jpg"",
                    ""Meta"": [],
                    ""Name"": ""lemon zest"",
                    ""Original"": ""1 Zest of lemon"",
                    ""OriginalName"": ""Zest of lemon"",
                    ""Unit"": """",
                    ""UnitLong"": """",
                    ""UnitShort"": """"
                }
            ],
            ""Title"": ""Easy & Delish! ~ Apple Crumble"",
            ""UnusedIngredients"": [],
            ""UsedIngredientCount"": 1,
            ""UsedIngredients"": [
                {
                    ""Aisle"": ""Produce"",
                    ""Amount"": 3,
                    ""Id"": 9003,
                    ""Image"": ""https://img.spoonacular.com/ingredients_100x100/apple.jpg"",
                    ""Meta"": [
                        ""sliced""
                    ],
                    ""Name"": ""apples"",
                    ""Original"": ""3 apples – sliced"",
                    ""OriginalName"": ""apples – sliced"",
                    ""Unit"": """",
                    ""UnitLong"": """",
                    ""UnitShort"": """"
                }
            ]
        }
    ],
    ""Success"": true
}

";

                var response = JsonSerializer.Deserialize<ListResponse<Recipe>>(r);
#else
                var response = await client.SearchRecipesByIngredients(queryPrams);
#endif
                if (!response.Success)
                    throw new Exception(response.Message);

                var storage = storageFactory.GetStorage();

                // Use transaction so we Rollback everything if we could not add a record. we don't want to come up with a broken record in a DB
                // Maybe we get a connection per recipe??
                using (var conn = storage.OpenConnection())
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var recipe in response.Objects)
                        {
                            await storage.AddRecipe(conn, recipe);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error during saving Recipes in database");
                        transaction.Rollback();
                    }
                    
                }

              


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
