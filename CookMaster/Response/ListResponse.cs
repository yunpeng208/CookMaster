using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Response
{
    public class ListResponse<T>: ApplicationResponse
    {
        public List<T> Objects { get; set; }
    }
}
