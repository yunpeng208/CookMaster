using CookMaster.Interfaces;
using CookMaster.Models;
using CookMaster.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CookMaster.Services.Clients
{
    public class SpoonacularClient: ISpoonacularClient
    {

        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public SpoonacularClient(HttpClient httpClient, ILoggerFactory loggerFactory)
        {
            this.httpClient = httpClient;
            this.logger = loggerFactory.CreateLogger<SpoonacularClient>();

            this.jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // Match property names ignoring case
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Expect camelCase JSON
            };
        }

        public async Task<ListResponse<RecipeSearchResult>> SearchRecipesByIngredients(Dictionary<string, string> queryPrams)
        {
            var uri = BuildUri("recipes/findByIngredients", queryPrams);

            using (var req = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                var response = await httpClient.SendAsync(req);

                if (response.IsSuccessStatusCode)
                {

                    var result = await response.Content.ReadFromJsonAsync<List<RecipeSearchResult>>(jsonSerializerOptions);

                    return new ListResponse<RecipeSearchResult> ()
                    {
                        Success = true,
                        Objects = result
                    };
                }

                throw await HandleError(response);
            }
        }

        public async Task<SingletonResponse<NutritionInfo>> GetRecipeNutritions(int recipeID)
        {
            var uri = BuildUri($"recipes/{recipeID}/nutritionWidget.json");

            using (var req = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                var response = await httpClient.SendAsync(req);

                if (response.IsSuccessStatusCode)
                {

                    var result = await response.Content.ReadFromJsonAsync<NutritionInfo>(jsonSerializerOptions);
                    result.RecipeID = recipeID;

                    return new SingletonResponse<NutritionInfo>()
                    {
                        Success = true,
                        Object = result
                    };
                }

                throw await HandleError(response);
            }
        }

        public async Task<SingletonResponse<Recipe>> GetRecipeInformation(int recipeID)
        {
            var uri = BuildUri($"recipes/{recipeID}/information");

            using (var req = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                var response = await httpClient.SendAsync(req);

                if (response.IsSuccessStatusCode)
                {

                    var result = await response.Content.ReadFromJsonAsync<Recipe>(jsonSerializerOptions);
                    return new SingletonResponse<Recipe>()
                    {
                        Success = true,
                        Object = result
                    };
                }

                throw await HandleError(response);
            }
        }


        #region Helpers
        private  Uri BuildUri(string path, IDictionary<string, string> query = null)
        {
            var sb = new StringBuilder();
            sb.Append(path);

            if (query != null && query.Any())
            {
                var q = query
                .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}")
                .ToArray();

                if (q.Length > 0)
                {
                    sb.Append('?');
                    sb.Append(string.Join("&", q));
                }
                sb.Append("&number=2");
            }

            return new Uri(sb.ToString(), UriKind.Relative);
        }

        private async Task<Exception> HandleError(HttpResponseMessage response)
        {
            string errorMsg = null;
            string body = null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                errorMsg = response.StatusCode.ToString();
            }
            else
            {
                try
                {
                    body = await response.Content.ReadAsStringAsync();
                }
                catch
                {
                }
            }

            throw new Exception(string.Format("HTTP Call '{0}' failed with {1}. Details: {2}", response.RequestMessage.RequestUri, response.StatusCode, errorMsg != null ? errorMsg : body));
        }
        #endregion
    }
}
