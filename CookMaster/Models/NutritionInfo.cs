using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class NutritionInfo
    {
        [PK]
        public int RecipeID { get; set; }

        public string Calories { get; set; }
        public string Carbs { get; set; }
        public string Fat { get; set; }
        public string Protein { get; set; }

        [NotStoredInDB]
        public List<BadGoodItem> Bad { get; set; }

        [NotStoredInDB]
        public List<BadGoodItem> Good { get; set; }

        [NotStoredInDB]
        public List<Nutrient> Nutrients { get; set; }

        [NotStoredInDB]
        public List<Property> Properties { get; set; }

        [NotStoredInDB]
        public List<Flavonoid> Flavonoids { get; set; }

        [NotStoredInDB]
        public List<Ingredient> Ingredients { get; set; }

        [NotStoredInDB]
        public CaloricBreakdown CaloricBreakdown { get; set; }

        [NotStoredInDB]
        public WeightPerServing WeightPerServing { get; set; }

        [NotStoredInDB]
        public long Expires { get; set; }

        [NotStoredInDB]
        public bool Stale { get; set; }
    }
}
