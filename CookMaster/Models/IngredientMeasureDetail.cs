using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public enum IngredientMeasureDetailTypes: byte
    {
        None = 0,
        US = 1,
        Metric = 2
    }
    public class IngredientMeasureDetail
    {
        [PK]
        public int RecipeID { get; set; }
        [PK]
        public int IngredientID { get; set; }

        public double Amount { get; set; }
        public string UnitLong { get; set; }
        public string UnitShort { get; set; }
        public IngredientMeasureDetailTypes Type { get; set; }
    }
}
