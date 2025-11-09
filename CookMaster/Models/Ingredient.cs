using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
        public List<Nutrient> Nutrients { get; set; }
    }
}
