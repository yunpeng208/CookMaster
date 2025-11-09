using System.Collections.Generic;

namespace CookMaster.Models
{
    public class Recipe
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public string ImageType { get; set; }
        public int Likes { get; set; }
        public int MissedIngredientCount { get; set; }
        public List<IngredientItem> MissedIngredients { get; set; }
        public string Title { get; set; }
        public List<IngredientItem> UnusedIngredients { get; set; }
        public int UsedIngredientCount { get; set; }
        public List<IngredientItem> UsedIngredients { get; set; }
    }
}
