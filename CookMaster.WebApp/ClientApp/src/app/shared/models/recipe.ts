import {Ingredient} from './ingredient';
import {RecipeInstruction} from './recipe-instruction';

export interface Recipe {
  ID: number;
  Title: string;
  Image: string;
  ImageType: string;
  Servings: number | null;
  ReadyInMinutes: number | null;
  License: string;
  SourceName: string;
  SourceUrl: string;
  SpoonacularSourceUrl: string;
  AggregateLikes: number | null;
  HealthScore: number | null;
  SpoonacularScore: number | null;
  PricePerServing: number | null;

  Cheap: boolean | null;
  CreditsText: string;

  DairyFree: boolean | null;
  Gaps: string;
  GlutenFree: boolean | null;
  Instructions: string;
  Ketogenic: boolean | null;
  LowFodmap: boolean | null;
  Sustainable: boolean | null;
  Vegan: boolean | null;
  Vegetarian: boolean | null;
  VeryHealthy: boolean | null;
  VeryPopular: boolean | null;
  Whole30: boolean | null;
  WeightWatcherSmartPoints: number | null;
  Summary: string;
  Likes: number | null;

  DishTypes: string[];
  Cuisines: string[];
  Diets: string[];
  Occasions: string[];

  ExtendedIngredients: Ingredient[];
  AnalyzedInstructions: RecipeInstruction[];
}
