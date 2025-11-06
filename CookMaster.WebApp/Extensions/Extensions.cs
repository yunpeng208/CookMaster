using Microsoft.AspNetCore.Http;

namespace CookMaster.WebApp.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Get current client public IP address
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRemoteIPAddress(this HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("x-forwarded-for", out var value))
            {
                return value.ToString();
            }

            return context.Connection.RemoteIpAddress.ToString();
        }
    }
}
