-- ======================================================
-- Database: cook_master_db
-- ======================================================

CREATE DATABASE "cook_master_db";

---------------------------------------------------------
-- Schema
---------------------------------------------------------
CREATE SCHEMA IF NOT EXISTS "CookMaster";


---------------------------------------------------------
-- Recipes
---------------------------------------------------------
CREATE TABLE "CookMaster"."Recipes" (
    "ID" INT PRIMARY KEY,
    "Title" TEXT,
    "Image" TEXT,
    "ImageType" TEXT,
    "Servings" INT,
    "ReadyInMinutes" INT,
    "License" TEXT,
    "SourceName" TEXT,
    "SourceUrl" TEXT,
    "SpoonacularSourceUrl" TEXT,
    "AggregateLikes" INT,
    "HealthScore" DOUBLE PRECISION,
    "SpoonacularScore" DOUBLE PRECISION,
    "PricePerServing" DOUBLE PRECISION,

    "Cheap" BOOLEAN,
    "CreditsText" TEXT,

    "DairyFree" BOOLEAN,
    "Gaps" TEXT,
    "GlutenFree" BOOLEAN,
    "Instructions" TEXT,
    "Ketogenic" BOOLEAN,
    "LowFodmap" BOOLEAN,
    "Sustainable" BOOLEAN,
    "Vegan" BOOLEAN,
    "Vegetarian" BOOLEAN,
    "VeryHealthy" BOOLEAN,
    "VeryPopular" BOOLEAN,
    "Whole30" BOOLEAN,
    "WeightWatcherSmartPoints" INT,
    "Likes" INT NULL,
    "Summary" TEXT
);

---------------------------------------------------------
-- RecipeInstructionSteps
---------------------------------------------------------

CREATE TABLE "CookMaster"."RecipeInstructionSteps" (
    "ID" UUID PRIMARY KEY,
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "Number" INT,
    "Step" TEXT
);

---------------------------------------------------------
-- RecipesDishTypes
---------------------------------------------------------

CREATE TABLE "CookMaster"."RecipesDishTypes" (
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "DishType" TEXT,
    PRIMARY KEY ("RecipeID", "DishType")
);


---------------------------------------------------------
-- RecipesCuisines
---------------------------------------------------------

CREATE TABLE "CookMaster"."RecipesCuisines" (
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "Cuisine" TEXT,
    PRIMARY KEY ("RecipeID", "Cuisine")
);


---------------------------------------------------------
-- RecipesDiets
---------------------------------------------------------

CREATE TABLE "CookMaster"."RecipesDiets" (
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "Diet" TEXT,
    PRIMARY KEY ("RecipeID", "Diet")
);

---------------------------------------------------------
-- RecipesOccasions
---------------------------------------------------------

CREATE TABLE "CookMaster"."RecipesOccasions" (
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "Occasion" TEXT,
    PRIMARY KEY ("RecipeID", "Occasion")
);

---------------------------------------------------------
-- Ingredients
---------------------------------------------------------
CREATE TABLE "CookMaster"."Ingredients" (
    "ID" INT PRIMARY KEY,
    "Name" TEXT,
    "Aisle" TEXT,
    "Consitency" TEXT,
    "Image" TEXT,
    "Original" TEXT,
    "OriginalName" TEXT,
    "Unit" TEXT
);

-- Index for fast lookup by ingredient name
CREATE INDEX "IX_Ingredients_Name"
    ON "CookMaster"."Ingredients" ("Name");

---------------------------------------------------------
-- IngredientMetas (Meta array items)
---------------------------------------------------------

CREATE TABLE "CookMaster"."IngredientMetas" (
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "IngredientID" INT REFERENCES "CookMaster"."Ingredients"("ID"),
    "Meta" TEXT,
    PRIMARY KEY ("RecipeID", "IngredientID", "Meta")
);


---------------------------------------------------------
-- RecipesIngredients
---------------------------------------------------------

CREATE TABLE "CookMaster"."RecipesIngredients" (
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "IngredientID" INT REFERENCES "CookMaster"."Ingredients"("ID"),
    "Amount" DOUBLE PRECISION,
    PRIMARY KEY ("RecipeID", "IngredientID")
);



---------------------------------------------------------
-- IngredientMeasureDetails
---------------------------------------------------------
CREATE TABLE "CookMaster"."IngredientMeasureDetails" (
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "IngredientID" INT REFERENCES "CookMaster"."Ingredients"("ID"),
    "Amount" DOUBLE PRECISION,
    "UnitLong" TEXT,
    "UnitShort" TEXT,
    "Type"    SMALLINT NOT NULL DEFAULT 0
);


---------------------------------------------------------
-- NutritionInfo
---------------------------------------------------------

CREATE TABLE "CookMaster"."NutritionInfos" (
    "RecipeID" INT PRIMARY KEY REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "Calories" TEXT,
    "Carbs" TEXT,
    "Fat" TEXT,
    "Protein" TEXT
);

---------------------------------------------------------
-- WeightPerServing
---------------------------------------------------------

CREATE TABLE "CookMaster"."WeightPerServings" (
    "RecipeID" INT PRIMARY KEY REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "Amount" DOUBLE PRECISION,
    "Unit" TEXT
);

---------------------------------------------------------
-- BadGoodItem
---------------------------------------------------------
CREATE TABLE "CookMaster"."BadGoodItems" (
    "ID" UUID PRIMARY KEY,
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "Title" TEXT,
    "Amount" TEXT,
    "Indented" BOOLEAN,
    "PercentOfDailyNeeds" DOUBLE PRECISION,
    "Type"    SMALLINT NOT NULL DEFAULT 0
);

---------------------------------------------------------
-- CaloricBreakdown
---------------------------------------------------------

CREATE TABLE "CookMaster"."CaloricBreakdowns" (
    "RecipeID" INT PRIMARY KEY REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "PercentProtein" DOUBLE PRECISION,
    "PercentFat" DOUBLE PRECISION,
    "PercentCarbs" DOUBLE PRECISION
);

---------------------------------------------------------
-- Flavonoid
---------------------------------------------------------

CREATE TABLE "CookMaster"."Flavonoids" (
    "ID" UUID PRIMARY KEY,
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "Name" TEXT,
    "Amount" DOUBLE PRECISION,
    "Unit" TEXT
);

---------------------------------------------------------
-- Nutrient
---------------------------------------------------------

CREATE TABLE "CookMaster"."Nutrients" (
    "ID" UUID PRIMARY KEY,
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "Name" TEXT,
    "Amount" DOUBLE PRECISION,
    "Unit" TEXT,
    "PercentOfDailyNeeds" DOUBLE PRECISION
);

---------------------------------------------------------
-- Property
---------------------------------------------------------

CREATE TABLE "CookMaster"."Properties" (
    "ID" UUID PRIMARY KEY,
    "RecipeID" INT REFERENCES "CookMaster"."Recipes"("ID") ON DELETE CASCADE,
    "Name" TEXT,
    "Amount" DOUBLE PRECISION,
    "Unit" TEXT
);

---------------------------------------------------------
-- IngredientsNutrient
---------------------------------------------------------

CREATE TABLE "CookMaster"."IngredientsNutrients" (
    "ID" UUID PRIMARY KEY,
    "IngredientID" INT REFERENCES "CookMaster"."Ingredients"("ID") ON DELETE CASCADE,
    "Name" TEXT,
    "Amount" DOUBLE PRECISION,
    "Unit" TEXT,
    "PercentOfDailyNeeds" DOUBLE PRECISION
);