export interface IngredientsNutrient {
  ID: string;           // Guid -> string
  IngredientID: number;

  Name: string;
  Amount: number;
  Unit: string;
  PercentOfDailyNeeds: number;
}
