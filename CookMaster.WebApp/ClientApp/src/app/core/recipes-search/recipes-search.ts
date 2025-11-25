import { Component } from '@angular/core';
import {RecipeSearchResult} from '../../shared/models/recipe-search-result';
import {RecipesService} from '../../shared/services/recipes.service';
import {FormsModule} from '@angular/forms';
import {Recipe} from '../../shared/models/recipe';
import {RecipeDetails} from '../recipe-details/recipe-details';

@Component({
  selector: 'app-recipes-search',
  imports: [
    FormsModule,
    RecipeDetails
  ],
  templateUrl: './recipes-search.html',
  styleUrl: './recipes-search.scss',
})
export class RecipesSearch {

  ingredients = '';
  recipes: RecipeSearchResult[] = [];
  isLoading = false;
  errorMessage: string | null = null;

  selectedRecipe: Recipe | null = null;

  constructor(private recipesService: RecipesService) {}

  onSearch(): void {
    this.errorMessage = null;

    const trimmed = this.ingredients.trim();
    if (!trimmed) {
      this.errorMessage = 'Please enter at least one ingredient.';
      this.recipes = [];
      return;
    }

    this.isLoading = true;

    this.recipesService.searchByIngredients(trimmed).subscribe({
        next: res => {
          if (res.Success) {
            this.recipes = res.Objects ?? [];
          } else {
            console.error(res.Message);
            this.errorMessage = 'Failed to load recipes. Please try again.';
          }

          this.isLoading = false;
        },
        error: err => {
          console.error(err);
          this.errorMessage = 'Failed to load recipes. Please try again.';
          this.isLoading = false;
        }
      });
  }

  showDetails(recipeID: number): void {
    this.isLoading = true;

    this.recipesService.getRecipeInformation(recipeID).subscribe({
      next: res => {
        if (res.Success) {
          this.selectedRecipe = res.Object
        } else {
          console.error(res.Message);
          this.errorMessage = res.Message;
        }

        console.log(this.selectedRecipe);

        this.isLoading = false;
      },
      error: err => {
        console.error(err);
        this.errorMessage = `Failed to load recipe ${recipeID}. Please try again.`;
        this.isLoading = false;
      }
    });
  }

  backToSearch() {
    this.selectedRecipe = null;
  }
}
