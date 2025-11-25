using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class Ingredient
    {
        [PK]
        public int ID { get; set; }

        public string Aisle { get; set; }

        [NotStoredInDB]
        public double Amount { get; set; }

         public string Consitency { get; set; }

        public string Image { get; set; }
        public string Name { get; set; }
        public string Original { get; set; }
        public string OriginalName { get; set; }
        public string Unit { get; set; }

        [NotStoredInDB]
        public List<string> Meta { get; set; }

        [NotStoredInDB]
        public IngredientMeasure Measures { get; set; }

        [NotStoredInDB]
        public List<IngredientsNutrient> Nutrients { get; set; }
    }
}
