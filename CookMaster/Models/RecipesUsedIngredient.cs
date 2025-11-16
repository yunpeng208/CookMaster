using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class RecipesUsedIngredient
    {
        [PK]
        public int RecipeID { get; set; }
        [PK]
        public Guid IngredientItemID { get; set; }
    }
}
