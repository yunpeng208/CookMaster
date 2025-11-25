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
        public async Task CacheRecipes(IDbConnection conn, List<RecipeSearchResult> results)
        {
            foreach (var result in results)
            {
                try
                {
                    var recipeID = await AddRecordOrSkipAsync(conn, new Recipe() { 
                        
                        ID = result.ID,
                        Image = result.Image,
                        ImageType = result.ImageType,
                        Likes = result.Likes,
                        Title = result.Title,


                        //Servings = 0,
                        //ReadyInMinutes = 0,
                        //License 
                        //SourceName 
                        //SourceUrl 
                        //SpoonacularSourceUrl 
                        //AggregateLikes 
                        //HealthScore 
                        //SpoonacularScore 
                        //PricePerServing 

                        //Cheap 
                        //CreditsText 

                        //DairyFree 
                        //Gaps 
                        //GlutenFree 
                        //nstructions 
                        //Ketogenic 
                        //LowFodmap 
                        //Sustainable 
                        //Vegan 
                        //Vegetarian 
                        //VeryHealthy 
                        //VeryPopular 
                        //Whole30 
                        //WeightWatcherSmartPoints 
                        //Summary 

                        
                    }, new List<string>() { "ID" });

                    // We already have this record in DB
                    if (!recipeID.HasValue)
                        continue;

                    if (recipeID.Value == 0)
                        throw new Exception($"Failed to Cache Recipe {result.ID} in database");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during caching recipe {0}", result.ID);
                }
            }
    
        }
        public async Task SaveRecipe(IDbConnection conn, Recipe recipe)
        {
            var recipeID = await AddOrUpdateRecordAsync(conn, recipe, new List<string>() { "ID" });

            // We already have this record in DB
            if (!recipeID.HasValue)
                return;

            if (recipeID.Value == 0)
                throw new Exception($"Failed to Save Recipe {recipe.ID} in database");

            foreach (var dishType in recipe.DishTypes)
            {
                var recipesDishType = new RecipesDishType()
                {
                    RecipeID = recipe.ID,
                    DishType = dishType,
                };

                var affectedRows = await AddRecordAsync(conn, recipesDishType);

                if (affectedRows <= 0)
                    throw new Exception($"Failed to save DishType {dishType} in database for recipe {recipeID.Value}");
            }

            foreach (var cuisine in recipe.Cuisines)
            {
                var recipesCuisine = new RecipesCuisine()
                {
                    RecipeID = recipe.ID,
                    Cuisine = cuisine,
                };

                var affectedRows = await AddRecordAsync(conn, recipesCuisine);

                if (affectedRows <= 0)
                    throw new Exception($"Failed to save Cuisine {cuisine} in database for recipe {recipeID.Value}");
            }

            foreach (var diet in recipe.Diets)
            {
                var recipesDiet = new RecipesDiet()
                {
                    RecipeID = recipe.ID,
                    Diet = diet,
                };

                var affectedRows = await AddRecordAsync(conn, recipesDiet);

                if (affectedRows <= 0)
                    throw new Exception($"Failed to save Diet {diet} in database for recipe {recipeID.Value}");
            }

            foreach (var occasion in recipe.Occasions)
            {
                var recipesOccasion = new RecipesOccasion()
                {
                    RecipeID = recipe.ID,
                    Occasion = occasion,
                };

                var affectedRows = await AddRecordAsync(conn, recipesOccasion);

                if (affectedRows <= 0)
                    throw new Exception($"Failed to save Occasion {occasion} in database for recipe {recipeID.Value}");
            }

            foreach (var instruction in recipe.AnalyzedInstructions)
            {
                foreach(var step in  instruction.Steps)
                {
                    var recipeInstructionStep = new RecipeInstructionStep()
                    {
                        ID = Guid.NewGuid(),
                        RecipeID = recipe.ID,
                        Number = step.Number,
                        Step = step.Step
                    };

                    var affectedRows = await AddRecordAsync(conn, recipeInstructionStep);

                    if (affectedRows <= 0)
                        throw new Exception($"Failed to save Instruction Step {recipeInstructionStep.Step} in database for recipe {recipeID.Value}");
                }
   
            }

            foreach (var ingredient in recipe.ExtendedIngredients)
            {
                await SaveRecipeIngredient(conn, ingredient, recipe.ID);
            }

        }
        public async Task<Recipe> GetRecipe(IDbConnection conn, int recipeID, bool full = false)
        {
            var record = await GetRecordsAsync<Recipe>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "ID", PropertyValue = recipeID } });

            var recipe = record.FirstOrDefault();

            if (recipe == null)
                return null;

            if (!full)
                return recipe;

            #region DishTypes
            var dishTypes = await GetRecordsAsync<RecipesDishType>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID } });

            recipe.DishTypes = dishTypes.Select(x => x.DishType).ToList();
            #endregion

            #region Cuisines
            var cuisines = await GetRecordsAsync<RecipesCuisine>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID } });

            recipe.Cuisines = cuisines.Select(x => x.Cuisine).ToList();
            #endregion

            #region Occasions
            var occasions = await GetRecordsAsync<RecipesOccasion>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID } });

            recipe.Occasions = occasions.Select(x => x.Occasion).ToList();
            #endregion

            #region Diets
            var diets = await GetRecordsAsync<RecipesDiet>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID } });

            recipe.Diets = diets.Select(x => x.Diet).ToList();
            #endregion

            #region Steps
            var steps = await GetRecordsAsync<RecipeInstructionStep>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID } });

            recipe.AnalyzedInstructions = new List<RecipeInstruction>()
            {
                new RecipeInstruction()
                {
                    Name = "",
                    Steps = steps
                }
            };
            #endregion

            #region ExtendedIngredients
            recipe.ExtendedIngredients = await GetRecipeIngredients(conn, recipeID);
            #endregion

            return recipe;
        }
        public async Task UpdateRecipe(IDbConnection conn, Recipe recipe)
        {
           await UpdateRecordAsync(conn, recipe);
        }

        private async Task<bool> SaveRecipeIngredient(IDbConnection conn, Ingredient ingredient, int recipeID)
        {
            var ingredientID = await AddRecordOrSkipAsync(conn, ingredient, new List<string>() { "ID" });

            // We already have this record in DB
            if (!ingredientID.HasValue)
                return true;

            if (ingredientID.Value == 0)
                throw new Exception($"Failed to Save Recipe {recipeID} Ingredient {ingredient.ID} in database");

            foreach(var meta in ingredient.Meta)
            {
                var ingredientMeta = new IngredientMeta()
                {
                    IngredientID = ingredient.ID,
                    RecipeID = recipeID,
                    Meta = meta,
                };

                var affectedRows = await AddRecordAsync(conn, ingredientMeta);

                if (affectedRows <= 0)
                    throw new Exception($"Failed to save Recipe {recipeID} Ingredient {ingredient.ID} Meta {meta} in database");

            }

            if(ingredient.Measures != null)
            {
                var measures = ingredient.Measures;

                if (measures.Us != null)
                {
                    var ingredientMeasureDetail = new IngredientMeasureDetail()
                    {
                        IngredientID = ingredient.ID,
                        RecipeID = recipeID,
                        Amount = measures.Us.Amount,
                        UnitLong = measures.Us.UnitLong,
                        UnitShort = measures.Us.UnitShort,
                        Type = IngredientMeasureDetailTypes.US
                    };

                    var affectedRows = await AddRecordAsync(conn, ingredientMeasureDetail);

                    if (affectedRows <= 0)
                        throw new Exception($"Failed to save Recipe {recipeID} Ingredient {ingredient.ID} Measure Us in database");
                }

                if (measures.Metric != null)
                {
                    var ingredientMeasureDetail = new IngredientMeasureDetail()
                    {
                        IngredientID = ingredient.ID,
                        RecipeID = recipeID,
                        Amount = measures.Metric.Amount,
                        UnitLong = measures.Metric.UnitLong,
                        UnitShort = measures.Metric.UnitShort,
                        Type = IngredientMeasureDetailTypes.Metric
                    };

                    var affectedRows = await AddRecordAsync(conn, ingredientMeasureDetail);

                    if (affectedRows <= 0)
                        throw new Exception($"Failed to save Recipe {recipeID} Ingredient {ingredient.ID} Measure Metric in database");
                }
            }


            var recipesIngredient = new RecipesIngredient()
            {
                IngredientID = ingredient.ID,
                RecipeID = recipeID,
                Amount = ingredient.Amount
            };

            var rows = await AddRecordAsync(conn, recipesIngredient);

            if (rows <= 0)
                throw new Exception($"Failed to save Recipe {recipeID} Ingredient {ingredient.ID} Mapping in database");

            return true;
    
        }
        private async Task<List<Ingredient>> GetRecipeIngredients(IDbConnection conn, int recipeID)
        {
            var cols = GetDbPropertyNames<Ingredient>();
            var ingredientsTable = TableName<Ingredient>();
            var colList = string.Join(", ", cols.Select(x => $"ing.{Q(x)}"));

            var recipesIngredientsTable = TableName<RecipesIngredient>();

            var sql = new StringBuilder();

            sql.AppendLine($@"
            SELECT {colList}
            FROM {ingredientsTable} ing
            JOIN {recipesIngredientsTable} rin ON rin.""IngredientID"" = ing.""ID""
            WHERE rin.""RecipeID"" = @RecipeID
            ");
            var ingredients = await conn.QueryAsync<Ingredient>(sql.ToString(), new { RecipeID = recipeID });

            var extendedIngredients = ingredients.ToList();


            foreach (var ing in extendedIngredients)
            {
                #region Meta
                var metas = await GetRecordsAsync<IngredientMeta>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID }, new FilterBy() { PropertyName = "IngredientID", PropertyValue = ing.ID } });

                ing.Meta = metas.Select(x => x.Meta).ToList();
                #endregion

                #region Measures
                var measures = await GetRecordsAsync<IngredientMeasureDetail>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID }, new FilterBy() { PropertyName = "IngredientID", PropertyValue = ing.ID } });

                ing.Measures = new IngredientMeasure()
                {
                    Metric = measures.FirstOrDefault(x => x.Type == IngredientMeasureDetailTypes.Metric),
                    Us = measures.FirstOrDefault(x => x.Type == IngredientMeasureDetailTypes.US)
                };
                #endregion
            }


            return extendedIngredients;
        }

        public async Task<List<RecipeSearchResult>> GetRecipesByIngredients(IDbConnection conn, List<string> ingredientNames)
        {
            var sql = @"WITH input_ingredients AS (
                    SELECT DISTINCT lower(unnest(@ingredientNames::text[])) AS name
                ),
                recipe_ingredients AS (
                    SELECT
                        r.""ID""                        AS recipe_id,
                        r.""Image""                     AS recipe_image,
                        r.""ImageType""                 AS recipe_imagetype,
                        r.""Likes""                     AS recipe_likes,
                        r.""Title""                     AS recipe_title,

                        ing.""ID""                      AS ingredient_id,
                        ing.""Aisle""                   AS ingredient_aisle,
                        ri.""Amount""                   AS ingredient_amount,
                        ing.""Image""                   AS ingredient_image,
                        ing.""Name""                    AS ingredient_name,
                        ing.""Original""                AS ingredient_original,
                        ing.""OriginalName""            AS ingredient_originalname,
                        ing.""Unit""                    AS ingredient_unit
                    FROM ""CookMaster"".""Recipes"" r
                    JOIN ""CookMaster"".""RecipesIngredients"" ri
                        ON ri.""RecipeID"" = r.""ID""
                    JOIN ""CookMaster"".""Ingredients"" ing
                        ON ing.""ID"" = ri.""IngredientID""
                    JOIN input_ingredients ii
                        ON lower(ing.""Name"") LIKE '%' || ii.name || '%'
                ),
                recipe_counts AS (
                    SELECT
                        recipe_id,
                        COUNT(*) AS ""UsedIngredientCount""
                    FROM recipe_ingredients
                    GROUP BY recipe_id
                )
                SELECT
                    ri.recipe_id                 AS ""ID"",
                    ri.recipe_image              AS ""Image"",
                    ri.recipe_imagetype          AS ""ImageType"",
                    ri.recipe_likes              AS ""Likes"",
                    ri.recipe_title              AS ""Title"",
                    rc.""UsedIngredientCount""   AS ""UsedIngredientCount"",

                    ri.ingredient_id             AS ""IngredientID"",  -- splitOn
                    ri.ingredient_id             AS ""ID"",
                    ri.ingredient_aisle          AS ""Aisle"",
                    ri.ingredient_amount         AS ""Amount"",
                    ri.ingredient_image          AS ""Image"",
                    ri.ingredient_name           AS ""Name"",
                    ri.ingredient_original       AS ""Original"",
                    ri.ingredient_originalname   AS ""OriginalName"",
                    ri.ingredient_unit           AS ""Unit""
                FROM recipe_ingredients ri
                JOIN recipe_counts rc
                    ON rc.recipe_id = ri.recipe_id
                ORDER BY
                    rc.""UsedIngredientCount"" DESC,
                    ri.recipe_id,
                    ri.ingredient_name;";

            var recipeDict = new Dictionary<int, RecipeSearchResult>();

            var rows = await conn.QueryAsync<RecipeSearchResult, IngredientSearchItem, RecipeSearchResult>(
                sql,
                (recipe, ingredient) =>
                {
                    if (!recipeDict.TryGetValue(recipe.ID, out var r))
                    {
                        r = recipe;
                        r.UsedIngredients = new List<IngredientSearchItem>();
                        recipeDict.Add(r.ID, r);
                    }

                    r.UsedIngredients.Add(ingredient);
                    return r;
                },
                new { ingredientNames },
                splitOn: "IngredientID" // important! matches the extra column we added
            );

            return recipeDict.Values.ToList();
        }
        #endregion

        #region RecipesNutritions
        public async Task SaveRecipeNutritions(IDbConnection conn, NutritionInfo nutritionInfo)
        {
            var nInfo = await AddRecordOrSkipAsync(conn, nutritionInfo, new List<string>() { "RecipeID" });

            // We already have this record in DB
            if (!nInfo.HasValue)
                return;

            if (nInfo.Value == 0)
                throw new Exception($"Failed to Save NutritionInfo For Recipe {nutritionInfo.RecipeID} in database");

            if (nInfo.HasValue && nInfo.Value == 0)
                throw new Exception("Failed to Add NutritionInfo");



            nutritionInfo.CaloricBreakdown.RecipeID = nutritionInfo.RecipeID;

            var nCaloricBreakdown = await AddRecordAsync(conn, nutritionInfo.CaloricBreakdown);

            if (nCaloricBreakdown == 0)
                throw new Exception($"Failed to Add CaloricBreakdown For Recipe {nutritionInfo.RecipeID} in database");

            nutritionInfo.WeightPerServing.RecipeID = nutritionInfo.RecipeID;

            var nWeightPerServing = await AddRecordAsync(conn, nutritionInfo.WeightPerServing);

            if (nWeightPerServing <= 0)
                throw new Exception($"Failed to Add WeightPerServing For Recipe {nutritionInfo.RecipeID} in database");

            foreach (var bad in nutritionInfo.Bad)
            {
                bad.RecipeID = nutritionInfo.RecipeID;
                bad.ID = Guid.NewGuid();
                bad.Type = BadGoodItemTypes.Bad;

                var affectedRows = await AddRecordAsync(conn, bad);

                if (affectedRows <= 0)
                    throw new Exception($"Failed to Add Bad Item For Recipe {nutritionInfo.RecipeID} in database");
            }

            foreach (var good in nutritionInfo.Good)
            {
                good.RecipeID = nutritionInfo.RecipeID;
                good.ID = Guid.NewGuid();
                good.Type = BadGoodItemTypes.Good;

                var affectedRows = await AddRecordAsync(conn, good);

                if (affectedRows <= 0)
                    throw new Exception("Failed to Add Good Item For Recipe {nutritionInfo.RecipeID} in database");
            }

            foreach (var nutrient in nutritionInfo.Nutrients)
            {
                nutrient.RecipeID = nutritionInfo.RecipeID;
                nutrient.ID = Guid.NewGuid();

                var affectedRows = await AddRecordAsync(conn, nutrient);

                if (affectedRows <= 0)
                    throw new Exception($"Failed to Add nutrient For Recipe {nutritionInfo.RecipeID} in database");
            }

            foreach (var flavonoid in nutritionInfo.Flavonoids)
            {
                flavonoid.RecipeID = nutritionInfo.RecipeID;
                flavonoid.ID = Guid.NewGuid();

                var affectedRows = await AddRecordAsync(conn, flavonoid);

                if (affectedRows <= 0)
                    throw new Exception($"Failed to Add flavonoid For Recipe {nutritionInfo.RecipeID} in database");
            }

            foreach (var ingredient in nutritionInfo.Ingredients)
            {
                foreach(var nutrient in ingredient.Nutrients)
                {
                    nutrient.IngredientID = ingredient.ID;
                    nutrient.ID = Guid.NewGuid();

                    var affectedRows = await AddRecordAsync(conn, nutrient);

                    if (affectedRows <= 0)
                        throw new Exception($"Failed to Add ingredient nutrient For Recipe {nutritionInfo.RecipeID} in database");
                }
             
            }
        }

        public async Task<NutritionInfo> GetRecipeNutritions(IDbConnection conn, int recipeID)
        {
            var record = await GetRecordsAsync<NutritionInfo>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID }});

            var nutritionInfo = record.FirstOrDefault();

            if (nutritionInfo == null)
                return null;

            #region CaloricBreakdown
            var caloricBreakdown = await GetRecordsAsync<CaloricBreakdown>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID } });
            nutritionInfo.CaloricBreakdown = caloricBreakdown.FirstOrDefault();
            #endregion

            #region WeightPerServing
            var weightPerServing = await GetRecordsAsync<WeightPerServing>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID } });
            nutritionInfo.WeightPerServing = weightPerServing.FirstOrDefault();
            #endregion

            #region Bad & Good Items
            var badGoodItems = await GetRecordsAsync<BadGoodItem>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID } });
            nutritionInfo.Bad = badGoodItems.Where(x => x.Type == BadGoodItemTypes.Bad).ToList();
            nutritionInfo.Good = badGoodItems.Where(x => x.Type == BadGoodItemTypes.Good).ToList();
            #endregion

            #region Nutrients
            var nutrients = await GetRecordsAsync<Nutrient>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID } });
            nutritionInfo.Nutrients = nutrients.ToList();
            #endregion

            #region Flavonoids
            var flavonoid = await GetRecordsAsync<Flavonoid>(conn, new List<FilterBy>() { new FilterBy() { PropertyName = "RecipeID", PropertyValue = recipeID } });
            nutritionInfo.Flavonoids = flavonoid.ToList();
            #endregion

            #region Ingredients
            nutritionInfo.Ingredients = await GetRecipeIngredients(conn, recipeID);
            #endregion

            return nutritionInfo;
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
            public PropertyInfo PropertyInfo { get; init; }
            public string Name { get; init; }             // C# property name == DB column name (PascalCase)
            public string SqlParamName { get; init; }     // e.g., @RecipeID
            public bool IsPrimaryKey { get; init; }
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


        private async Task<int> AddRecordAsync<T>(IDbConnection ctx, T record)
        {
            var cols = GetDbPropertyNames<T>();
            var table = TableName<T>();
            var colList = string.Join(", ", cols.Select(Q));
            var valList = string.Join(", ", cols.Select(c => "@" + c));
            var sql = $@"INSERT INTO {table} ({colList}) VALUES ({valList})";
            return await ctx.ExecuteAsync(sql, record);
        }

        internal sealed class FilterBy
        {
            public string PropertyName { get; set; }
            public object PropertyValue { get; set; }
        }

        private async Task<List<T>> GetRecordsAsync<T>(IDbConnection conn, List<FilterBy> filters = null)
        {
            var cols = GetDbPropertyNames<T>();
            var table = TableName<T>();
            var colList = string.Join(", ", cols.Select(Q));
            var sql = new StringBuilder();

            var parameter = new DynamicParameters();

            sql.AppendLine($@"
SELECT {colList}
FROM {table}
");

            if (filters != null && filters.Any())
            {
                var conition = "WHERE";
                foreach (var filter in filters)
                {
                    sql.AppendLine(conition);

                    conition = "AND";

                    sql.AppendLine($"{Q(filter.PropertyName)}=@{filter.PropertyName}");

                    parameter.Add($"@{filter.PropertyName}", filter.PropertyValue);

                }
            }
           
            var rows = await conn.QueryAsync<T>(sql.ToString(), parameter);

            return rows.ToList();
        }
        private async Task<int?> AddRecordOrSkipAsync<T>(IDbConnection conn, T record, List<string> conflictColumns)
        {
            var cols = GetDbPropertyNames<T>();
            var table = TableName<T>();
            var colList = string.Join(", ", cols.Select(Q));
            var valList = string.Join(", ", cols.Select(c => "@" + c));
            var sql = new StringBuilder();
            sql.AppendLine($@"INSERT INTO {table} ({colList}) VALUES ({valList})");

            var pk = GetPK<T>();
            if (conflictColumns == null)
            {
                conflictColumns = new List<string>() { pk.Name };
            }


            sql.AppendLine($@" ON CONFLICT ({string.Join(", ", conflictColumns.Select(Q))}) DO NOTHING");
 
            sql.AppendLine("RETURNING 1");

            return await conn.ExecuteScalarAsync<int?>(sql.ToString(), record);
        }
        private async Task<int> UpdateRecordAsync<T>(IDbConnection ctx, T record)
        {
            var props = GetDbProperties<T>();
            var pk = props.First(p => p.IsPrimaryKey);
            var setList = string.Join(", ", props.Where(p => !p.IsPrimaryKey).Select(p => $"{Q(p.Name)} = {p.SqlParamName}"));
            var table = TableName<T>();
            var sql = $"UPDATE {table} SET {setList} WHERE {Q(pk.Name)} = {pk.SqlParamName}";
            return await ctx.ExecuteAsync(sql, record);
        }

        private async Task<int?> AddOrUpdateRecordAsync<T>(IDbConnection conn,T record, List<string> conflictColumns)
        {
            var cols = GetDbPropertyNames<T>();
            var table = TableName<T>();

            var colList = string.Join(", ", cols.Select(Q));
            var valList = string.Join(", ", cols.Select(c => "@" + c));

            // Columns to update (everything except conflict columns)
            var updateCols = cols
                .Where(c => !conflictColumns.Contains(c))
                .Select(c => $"{Q(c)} = EXCLUDED.{Q(c)}");

            var pk = GetPK<T>();
            if (conflictColumns == null)
                conflictColumns = new List<string> { pk.Name };

            var sql = new StringBuilder();
            sql.AppendLine($@"INSERT INTO {table} ({colList}) VALUES ({valList})");

            sql.AppendLine($@"ON CONFLICT ({string.Join(", ", conflictColumns.Select(Q))}) DO UPDATE");
            sql.AppendLine($@"SET {string.Join(", ", updateCols)}");

            sql.AppendLine("RETURNING 1");

            return await conn.ExecuteScalarAsync<int?>(sql.ToString(), record);
        }

        #endregion
    }
}
