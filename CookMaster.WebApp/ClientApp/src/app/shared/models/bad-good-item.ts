import {BadGoodItemTypes} from '../enums/bad-good-item-types';

export interface BadGoodItem {
  ID: string;
  RecipeID: number;
  Title: string;
  Amount: string;
  Indented: boolean;
  PercentOfDailyNeeds: number;
  Type: BadGoodItemTypes;
}
