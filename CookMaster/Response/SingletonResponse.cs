using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Response
{
    public class SingletonResponse<T>: ApplicationResponse
    {
        public T Object { get; set; }
    }
}
