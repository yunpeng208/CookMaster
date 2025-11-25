using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class RecipeSearchResult
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public string ImageType { get; set; }
        public int Likes { get; set; }

        public int MissedIngredientCount { get; set; }
        public List<IngredientSearchItem> MissedIngredients { get; set; }

        public string Title { get; set; }

        public List<IngredientSearchItem> UnusedIngredients { get; set; }

        public int UsedIngredientCount { get; set; }
        public List<IngredientSearchItem> UsedIngredients { get; set; }
    }
}
