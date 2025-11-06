using CookMaster.Interfaces;
using CookMaster.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CookMaster.WebApp
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ILogger logger;

        public TestsController(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<TestsController>();
        }

        [HttpGet, Route("Test")]
        public ActionResult Test()
        {
            return Ok(new
            {
                Success = true
            });
        }

        [HttpGet, Route("Info")]
        public async Task<ActionResult> Info([FromServices] IStorageFactory storageFactory)
        {
            // Get storage using factory pattern
            var storage = storageFactory.GetStorage();
            var hasDB = "";


            try
            {
                // Open a connection to the Database
                using (var conn = storage.OpenConnection())
                {
                    // Check if the Database in the connection string exists 
                    hasDB = $"{conn.Database} {(await storage.DatabaseExistsAsync(conn) ? "exists" : "does not exist")}";
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error druing checking if Database exists or not");

                hasDB = $"Error druing checking if Database exists or not. Error: {ex.Message}";
            }

            return Ok(new
            {
                Success = true,
                MachineName = System.Environment.MachineName,
                RemoteIP = HttpContext.GetRemoteIPAddress(),
                OSDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                HasDB = hasDB
            });
        }
    }
}
