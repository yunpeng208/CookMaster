using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class IngredientMeasure
    {
        public IngredientMeasureDetail Metric { get; set; }
        public IngredientMeasureDetail Us { get; set; }
    }
}
