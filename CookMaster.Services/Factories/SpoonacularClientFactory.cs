using CookMaster.Interfaces;
using CookMaster.Services.Clients;
using CookMaster.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CookMaster.Services.Factories
{
    public class SpoonacularClientFactory: ISpoonacularClientFactory
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly AppConfig config;

        private SpoonacularClient spoonacularClient;
        private object sync = new();
        public SpoonacularClientFactory(ILoggerFactory loggerFactory, AppConfig config) { 
            this.loggerFactory = loggerFactory;
            this.config = config;
        }

        public ISpoonacularClient GetClient() {

            if (spoonacularClient != null)
                return spoonacularClient;

            if (config.Spoonacular == null)
                throw new ArgumentException("Spoonacular settings are required");

            if (String.IsNullOrEmpty(config.Spoonacular.BaseURL))
                throw new ArgumentException("Spoonacular BaseURL is required");

            if (String.IsNullOrEmpty(config.Spoonacular.APIKey))
                throw new ArgumentException("Spoonacular APIKey is required");

            lock(sync)
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(config.Spoonacular.BaseURL, UriKind.Absolute)
                };

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-api-key", config.Spoonacular.APIKey);

                spoonacularClient = new SpoonacularClient(client, loggerFactory);
            }

            return spoonacularClient;
        }
    }
}
