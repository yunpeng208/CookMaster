using System;
using System.Collections.Generic;

namespace CookMaster.Models
{
    public class Recipe
    {
        [PK]
        public int ID { get; set; }
        public string Image { get; set; }
        public string ImageType { get; set; }
        public int Likes { get; set; }
        public int MissedIngredientCount { get; set; }
        public string Title { get; set; }

        [NotStoredInDB]
        public int UsedIngredientCount { get; set; }

        [NotStoredInDB]
        public List<IngredientItem> UnusedIngredients { get; set; }
        [NotStoredInDB]
        public List<IngredientItem> MissedIngredients { get; set; }
        [NotStoredInDB]
        public List<IngredientItem> UsedIngredients { get; set; }
    }
}
