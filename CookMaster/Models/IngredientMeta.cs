using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class IngredientMeta
    {
        [PK]
        public int RecipeID { get; set; }
        [PK]
        public int IngredientID { get; set; }
        public string Meta { get; set; }
    }
}
