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
        public async Task<ActionResult<ListResponse<Recipe>>> RecipesByIngredients()
        {
            // Build Dictionary<string, string> from the query string (first value per key)
            var queryPrams = Request.Query.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToString());

            var response = await spoonacularService.RecipesByIngredients(queryPrams);

            return HandleResponse(response);
        }

        [HttpGet, Route("{RecipeID}/GetRecipesNutritions")]
        public async Task<ActionResult<ListResponse<NutritionInfo>>> GetRecipesNutritions(long RecipeID)
        {
            var response = await spoonacularService.GetRecipesNutritions(RecipeID);

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
