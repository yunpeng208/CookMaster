import {IngredientMeasure} from './ingredient-measure';
import {IngredientsNutrient} from './ingredients-nutrient';

export interface Ingredient {
  ID: number;
  Aisle: string;
  Amount: number;
  Consitency: string;  // kept typo to match backend exactly
  Image: string;
  Name: string;
  Original: string;
  OriginalName: string;
  Unit: string;

  Meta: string[];
  Measures: IngredientMeasure;
  Nutrients: IngredientsNutrient[];
}
