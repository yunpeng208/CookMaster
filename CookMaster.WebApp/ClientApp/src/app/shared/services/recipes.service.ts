import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {environment} from '../../../environments/environment';
import {Observable} from 'rxjs';
import {ListResponse} from '../responses/list-response';
import {RecipeSearchResult} from '../models/recipe-search-result';
import {SingletonResponse} from '../responses/singleton-response';
import {Recipe} from '../models/recipe';
import {NutritionInfo} from '../models/nutrition-info';

@Injectable({
  providedIn: 'root'
})
export class RecipesService {
  private readonly baseUrl: string;
  constructor(private http: HttpClient) {
    this.baseUrl = `${environment.appSettings.serverUrl}/api/Recipes`;
  }


  searchByIngredients(ingredients: string): Observable<ListResponse<RecipeSearchResult>> {
    const params = new HttpParams().set('ingredients', ingredients);
    return this.http.get<ListResponse<RecipeSearchResult>>(`${this.baseUrl}/RecipesByIngredients`, { params });
  }

  getRecipeNutritions(recipeID: number): Observable<SingletonResponse<NutritionInfo>> {
    return this.http.get<SingletonResponse<NutritionInfo>>(`${this.baseUrl}/${recipeID}/Nutritions`);
  }

  getRecipeInformation(recipeID: number): Observable<SingletonResponse<Recipe>> {
    return this.http.get<SingletonResponse<Recipe>>(`${this.baseUrl}/${recipeID}/information`);
  }
}
