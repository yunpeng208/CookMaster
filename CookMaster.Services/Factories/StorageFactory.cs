using CookMaster.Data;
using CookMaster.Interfaces;
using Microsoft.Extensions.Logging;

namespace CookMaster.Services.Factories
{
    public class StorageFactory: IStorageFactory
    {
        private readonly string connectionString;
        private readonly ILoggerFactory loggerFactory;

        public StorageFactory(string connectionStribg, ILoggerFactory loggerFactory)
        {
            this.connectionString = connectionStribg;
            this.loggerFactory = loggerFactory;
        }

        public IStorage GetStorage()
        {
            return new Storage(connectionString, loggerFactory);
        }
    }
}
