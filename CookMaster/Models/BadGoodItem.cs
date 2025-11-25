using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public enum BadGoodItemTypes : byte
    {
        None = 0,
        Bad = 1,
        Good = 2
    }
    public class BadGoodItem
    {
        [PK]
        public Guid ID { get; set; }
        public int RecipeID { get; set; }
        public string Title { get; set; }
        public string Amount { get; set; }
        public bool Indented { get; set; }
        public double PercentOfDailyNeeds { get; set; }
        public BadGoodItemTypes Type { get; set; }
    }
}
