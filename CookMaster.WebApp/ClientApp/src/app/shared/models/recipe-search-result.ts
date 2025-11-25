import {IngredientSearchItem} from './ingredient-search-item';

export interface RecipeSearchResult {
  ID: number;
  Image: string;
  ImageType: string;
  Likes: number;
  MissedIngredientCount: number;
  MissedIngredients: IngredientSearchItem[];
  Title: string;
  UnusedIngredients: IngredientSearchItem[];
  UsedIngredientCount: number;
  UsedIngredients: IngredientSearchItem[];
}
