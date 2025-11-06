using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Interfaces
{
    public interface IStorage
    {
        #region CTX
        IDbConnection OpenConnection(TimeSpan? timeout = null);
        #endregion


        Task<bool> DatabaseExistsAsync(IDbConnection conn);

    }
}
