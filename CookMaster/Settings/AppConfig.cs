using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Settings
{
    // TODO: Load settings from appsettings.json
    public class AppConfig
    {
        public string ApplicationURL { get; set; }
        public SpoonacularClientSettings Spoonacular {  get; set; }
    }
}
