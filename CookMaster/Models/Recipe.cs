using System;
using System.Collections.Generic;

namespace CookMaster.Models
{
    public class Recipe
    {
        [PK]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string ImageType { get; set; }
        public int? Servings { get; set; }
        public int? ReadyInMinutes { get; set; }
        public string License { get; set; }
        public string SourceName { get; set; }
        public string SourceUrl { get; set; }
        public string SpoonacularSourceUrl { get; set; }
        public int? AggregateLikes { get; set; }
        public double? HealthScore { get; set; }
        public double? SpoonacularScore { get; set; }
        public double? PricePerServing { get; set; }

        public bool? Cheap { get; set; }
        public string CreditsText { get; set; }

        public bool? DairyFree { get; set; }
        public string Gaps { get; set; }
        public bool? GlutenFree { get; set; }
        public string Instructions { get; set; }
        public bool? Ketogenic { get; set; }
        public bool? LowFodmap { get; set; }
        public bool? Sustainable { get; set; }
        public bool? Vegan { get; set; }
        public bool? Vegetarian { get; set; }
        public bool? VeryHealthy { get; set; }
        public bool? VeryPopular { get; set; }
        public bool? Whole30 { get; set; }
        public int? WeightWatcherSmartPoints { get; set; }
        public string Summary { get; set; }
        public int? Likes { get; set; }

        // Collections
        [NotStoredInDB]
        public List<string> DishTypes { get; set; }

        [NotStoredInDB]
        public List<string> Cuisines { get; set; }

        [NotStoredInDB]
        public List<string> Diets { get; set; }

        [NotStoredInDB]
        public List<string> Occasions { get; set; }

        [NotStoredInDB]
        public List<Ingredient> ExtendedIngredients { get; set; }

        [NotStoredInDB]
        public List<RecipeInstruction> AnalyzedInstructions { get; set; }
    }

    public class RecipeInstruction
    {
        public string Name { get; set; }
        public List<RecipeInstructionStep> Steps { get; set; }
    }

    public class RecipeInstructionStep
    {
        [PK]
        public Guid ID { get; set; }
        public int RecipeID { get; set; }
        public int Number { get; set; }
        public string Step { get; set; }
    }
}
