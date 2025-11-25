import {IngredientMeasureDetailTypes} from '../enums/ingredient-measure-detail-types';

export interface IngredientMeasureDetail {
  RecipeID: number;
  IngredientID: number;

  Amount: number;
  UnitLong: string;
  UnitShort: string;
  Type: IngredientMeasureDetailTypes;
}
