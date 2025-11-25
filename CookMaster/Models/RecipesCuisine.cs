using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class RecipesCuisine
    {
        [PK]
        public int RecipeID { get; set; }
        public string Cuisine { get; set; }
    }
}
