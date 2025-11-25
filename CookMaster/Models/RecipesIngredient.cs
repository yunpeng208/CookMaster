using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class RecipesIngredient
    {
        [PK]
        public int RecipeID { get; set; }
        [PK]
        public int IngredientID { get; set; }

        public double Amount { get; set; }
    }
}
