import {BadGoodItem} from './bad-good-item';
import {Nutrient} from './nutrient';
import {Property} from './property';
import {Flavonoid} from './flavonoid';
import {Ingredient} from './ingredient';
import {CaloricBreakdown} from './caloric-breakdown';
import {WeightPerServing} from './weight-per-serving';

export interface NutritionInfo {
  RecipeID: number;

  Calories: string;
  Carbs: string;
  Fat: string;
  Protein: string;

  Bad: BadGoodItem[];
  Good: BadGoodItem[];
  Nutrients: Nutrient[];
  Properties: Property[];
  Flavonoids: Flavonoid[];
  Ingredients: Ingredient[];

  CaloricBreakdown: CaloricBreakdown;
  WeightPerServing: WeightPerServing;

  Expires: number;
  Stale: boolean;
}
