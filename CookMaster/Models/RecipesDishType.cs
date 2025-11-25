using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class RecipesDishType
    {
        [PK]
        public int RecipeID { get; set; }
        public string DishType { get; set; }
    }
}
