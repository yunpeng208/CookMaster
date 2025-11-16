using System;
using System.Collections.Generic;


namespace CookMaster.Models
{
    public class IngredientItem
    {
        [PK]
        public Guid ID { get; set; }
        public string Aisle { get; set; }
        public double Amount { get; set; }
        public string ExtendedName { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Original { get; set; }
        public string OriginalName { get; set; }
        public string Unit { get; set; }

        public string UnitLong { get; set; }
        public string UnitShort { get; set; }
    }
}
