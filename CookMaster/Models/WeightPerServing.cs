using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class WeightPerServing
    {
        [PK]
        public int RecipeID { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }
}
