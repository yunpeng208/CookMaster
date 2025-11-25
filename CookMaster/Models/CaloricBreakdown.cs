using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class CaloricBreakdown
    {
        [PK]
        public int RecipeID { get; set; }
        public double PercentProtein { get; set; }
        public double PercentFat { get; set; }
        public double PercentCarbs { get; set; }
    }
}
