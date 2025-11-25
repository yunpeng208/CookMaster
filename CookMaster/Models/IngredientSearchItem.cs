using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class IngredientSearchItem
    {
        public string Aisle { get; set; }
        public double Amount { get; set; }

        public int ID { get; set; }
        public string Image { get; set; }

        public List<string> Meta { get; set; }

        public string Name { get; set; }

        public string Original { get; set; }
        public string OriginalName { get; set; }

        public string Unit { get; set; }
        public string UnitLong { get; set; }
        public string UnitShort { get; set; }

        public string ExtendedName { get; set; }
    }
}
