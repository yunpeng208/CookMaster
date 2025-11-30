using CookMaster.Data;
using CookMaster.Interfaces;
using Microsoft.Extensions.Logging;

namespace CookMaster.Services.Factories
{    
    /// <summary>
    /// DESIGN PATTERN: Factory 
    /// - Creates IStorage instances with the correct connection string and logger.
    /// - Centralizes how the storage layer (Repository) is constructed.
    /// </summary>
    public class StorageFactory: IStorageFactory
    {
        private readonly string connectionString;
        private readonly ILoggerFactory loggerFactory;

        public StorageFactory(string connectionString, ILoggerFactory loggerFactory)
        {
            this.connectionString = connectionString;
            this.loggerFactory = loggerFactory;
        }

        public IStorage GetStorage()
        {
            // Factory Pattern: Encapsulates creation of the Storage (Repository) object.
            return new Storage(connectionString, loggerFactory);
        }
    }
}
