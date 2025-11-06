using CookMaster.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Data
{
    public class Storage : IStorage
    {
        private readonly string connectionString;
        private readonly string schemaName;
        private readonly ILogger logger;
        public Storage(string connectionString, ILoggerFactory loggerFactory, string schemaName = "CookMaster")
        {
            this.connectionString = connectionString;
            this.schemaName = schemaName;
            this.logger = loggerFactory.CreateLogger<Storage>();
        }

        #region CTX
        /// <summary>
        /// Open Connection to PostgreSQL Database
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IDbConnection OpenConnection(TimeSpan? timeout = null)
        {
            var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                // We will create the tables under CookMaster schema so set the search_path to that. 
                // We will store everything in UTC. So we will set the time zone for the database session to UTC.
                // We will set the session’s application name, visible in PostgreSQL logs or monitoring tools to CookMaster.WebApp.
                cmd.CommandText = $"SET search_path TO {Q(schemaName)}, public; SET TIME ZONE 'UTC'; SET application_name = 'CookMaster.WebApp';";
                cmd.ExecuteNonQuery();

                if (timeout is { } t && t > TimeSpan.Zero)
                {
                    cmd.CommandText = $"SET statement_timeout = {(int)t.TotalMilliseconds}";
                    cmd.ExecuteNonQuery();
                }
            }
            return conn;
        }
        #endregion

        /// <summary>
        /// Check if the database in connection sring exists
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public async Task<bool> DatabaseExistsAsync(IDbConnection conn)
        {
            const string sql = "SELECT EXISTS (SELECT 1 FROM pg_database WHERE datname = @name);";
            bool exists = await conn.QuerySingleAsync<bool>(sql, new { name = conn.Database });

            return exists;
        }


        #region Helpers
        /// <summary>
        /// Fix Quoting so PostgreSQL follow Pascal casing
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        private string Q(string ident)
        {
            return $"\"{ident.Replace("\"", "\"\"")}\"";
        }

        /// <summary>
        /// Using Reflection get table name pluralize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string TableName<T>()
        {
            var type = typeof(T);
            string name = type.Name;
            if (name.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) || name.EndsWith("x", StringComparison.InvariantCultureIgnoreCase))
                name = $"{name}es";
            else if (name.EndsWith("y", StringComparison.InvariantCultureIgnoreCase))
                name = $"{name.Substring(0, name.Length - 1)}ies";
            else
                name = $"{name}s";

            return $"{Q(schemaName)}.{Q(name)}";
        }
        #endregion
    }
}
