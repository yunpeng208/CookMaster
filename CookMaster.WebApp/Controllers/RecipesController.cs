using CookMaster.Interfaces;
using CookMaster.Models;
using CookMaster.Response;
using CookMaster.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CookMaster.WebApp
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly ISpoonacularService spoonacularService;

        public RecipesController(ILoggerFactory loggerFactory, ISpoonacularService spoonacularService)
        {
            this.logger = loggerFactory.CreateLogger<RecipesController>();
            this.spoonacularService = spoonacularService;
        }

        [HttpGet, Route("RecipesByIngredients")]
        public async Task<ActionResult<ListResponse<RecipeSearchResult>>> RecipesByIngredients()
        {
            // Build Dictionary<string, string> from the query string (first value per key)
            var queryPrams = Request.Query.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToString());

            var response = await spoonacularService.RecipesByIngredients(queryPrams);

            return HandleResponse(response);
        }

        [HttpGet, Route("{RecipeID}/Nutritions")]
        public async Task<ActionResult<SingletonResponse<NutritionInfo>>> GetRecipeNutritions(int RecipeID)
        {
            var response = await spoonacularService.GetRecipeNutritions(RecipeID);

            return HandleResponse(response);
        }

        [HttpGet, Route("{RecipeID}/information")]
        public async Task<ActionResult<SingletonResponse<Recipe>>> GetRecipeInformation(int RecipeID)
        {
            var response = await spoonacularService.GetRecipeInformation(RecipeID);

            return HandleResponse(response);
        }


        #region helpers
        protected ActionResult<T> HandleResponse<T>(T response) where T : ApplicationResponse
        {
            if (response.Success)
                return Ok(response);

            return Conflict(response);
        }
        #endregion
    }
}
