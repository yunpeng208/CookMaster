using CookMaster.Interfaces;
using CookMaster.Models;
using CookMaster.Response;
using CookMaster.Settings;
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

        public async Task<ListResponse<Recipe>> SearchRecipesByIngredients(Dictionary<string, string> queryPrams)
        {
            var uri = BuildUri("recipes/findByIngredients", queryPrams);

            using (var req = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                var response = await httpClient.SendAsync(req);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<Recipe>>(jsonSerializerOptions);

                    return new ListResponse<Recipe> ()
                    {
                        Success = true,
                        Objects = result
                    };
                }


                throw  await HandleError(response);


            }
        }
        public async Task<ListResponse<NutritionInfo>> NutritionsByID(long recipeID)
        {
            var uri = BuildUri($"recipes/{recipeID}/nutritionWidget.json");

            using (var req = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                var response = await httpClient.SendAsync(req);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<NutritionInfo>>(jsonSerializerOptions);

                    return new ListResponse<NutritionInfo>()
                    {
                        Success = true,
                        Objects = result
                    };
                }


                throw await HandleError(response);


            }
        }


        #region Hepers
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
