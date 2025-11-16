using CookMaster.Interfaces;
using CookMaster.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

        #region Recipe
        public async Task AddRecipe(IDbConnection conn, Recipe recipe)
        {
            var affectedRows = await AddRecordAsync(conn, recipe);

            if (affectedRows <= 0)
                throw new Exception("Failed to Add Recipe");


            foreach (var ingredientItem in recipe.UsedIngredients)
            {
                // TODO: Need to check if ingredientItem alrady exists. We need to add a unique constraint on the name
                ingredientItem.ID = Guid.NewGuid();
                var affectedRows2 = await AddRecordAsync(conn, ingredientItem);

                if (affectedRows2 <= 0)
                    throw new Exception("Failed to Add IngredientItem");

                var affectedRows3 = await AddRecordAsync(conn, new RecipesUsedIngredient()
                {
                    RecipeID = recipe.ID,
                    IngredientItemID = ingredientItem.ID,
                });

                if (affectedRows2 <= 0)
                    throw new Exception("Failed to Add RecipesUsedIngredient");
            }

        }
        #endregion



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


        #region  Reflection helpers
        private sealed class PropertyDetails
        {
            public PropertyInfo PropertyInfo { get; set; }
            public string Name { get; set; }
            public string SqlParamName { get; set; }
            public bool IsPrimaryKey { get; set; }
        }

        private List<PropertyDetails> GetDbProperties<T>()
        {
            var type = typeof(T);
            var list = new List<PropertyDetails>();

            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (p.GetCustomAttribute<NotStoredInDBAttribute>() != null)
                    continue;

                var isPk = p.GetCustomAttribute<PKAttribute>() != null
                           || string.Equals(p.Name, "ID", StringComparison.Ordinal);

                list.Add(new PropertyDetails
                {
                    PropertyInfo = p,
                    Name = p.Name,
                    SqlParamName = "@" + p.Name,
                    IsPrimaryKey = isPk
                });
            }

            if (!list.Any(x => x.IsPrimaryKey))
                throw new Exception($"Failed to find Primary Key for {type.Name}");

            return list;
        }

        private List<string> GetDbPropertyNames<T>(bool skipPk = false)
        {
            var props = GetDbProperties<T>();
            if (skipPk) 
                props = props.Where(p => !p.IsPrimaryKey).ToList();

            return props.Select(p => p.Name).ToList();
        }

        private PropertyDetails GetPK<T>()
        {
            var props = GetDbProperties<T>();
            return props.FirstOrDefault(p => p.IsPrimaryKey);
        }
        #endregion


        private async Task<int> AddRecordAsync<T>(IDbConnection ctx, T record, bool allowConflict = false)
        {
            var cols = GetDbPropertyNames<T>();
            var table = TableName<T>();
            var colList = string.Join(", ", cols.Select(Q));
            var valList = string.Join(", ", cols.Select(c => "@" + c));
            var sql = $@"INSERT INTO {table} ({colList}) VALUES ({valList})";
            if (allowConflict)
            {
                var pk = GetPK<T>();
                sql += $" ON CONFLICT (\"{pk.Name}\") DO NOTHING";
            }
            return await ctx.ExecuteAsync(sql, record);
        }

        internal sealed class FilterBy
        {
            public string PropertyName { get; set; }
            public object PropertyValue { get; set; }
        }

        private async Task<List<T>> GetRecordsAsync<T>(IDbConnection ctx, List<FilterBy> filters = null)
        {
            var cols = GetDbPropertyNames<T>();
            var table = TableName<T>();
            var colList = string.Join(", ", cols.Select(Q));
            var sql = new StringBuilder();

            var parameter = new DynamicParameters();

            sql.AppendLine($@"
SELECT {cols}
FROM {table}
");

            if (filters != null && filters.Any())
            {
                var conition = "WHERE";
                foreach (var filter in filters)
                {
                    sql.AppendLine(conition);

                    conition = "AND";

                    sql.AppendLine($"{filter.PropertyName}=@{filter.PropertyName}");

                    parameter.Add($"@{filter.PropertyName}", filter.PropertyValue);

                }
            }
           
            var rows = await ctx.QueryAsync<T>(sql.ToString(), parameter);

            return rows.ToList();
        }

        #endregion
    }
}
