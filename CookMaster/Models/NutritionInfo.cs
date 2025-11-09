using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class NutritionInfo
    {
        public List<Nutrient> Nutrients { get; set; }
        public List<NutritionProperty> Properties { get; set; }
        public List<Flavonoid> Flavonoids { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public CaloricBreakdown CaloricBreakdown { get; set; }
        public WeightPerServing WeightPerServing { get; set; }
    }
}
