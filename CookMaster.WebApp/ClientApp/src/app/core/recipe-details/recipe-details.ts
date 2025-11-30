import {Component, Input} from '@angular/core';
import {RecipesService} from '../../shared/services/recipes.service';
import {NutritionInfo} from '../../shared/models/nutrition-info';
import {Recipe} from '../../shared/models/recipe';
import {DecimalPipe} from '@angular/common';

// Angular "View" component for showing a single recipe.
// Displays data provided by the RecipesService.
@Component({
  selector: 'app-recipe-details',
  imports: [
    DecimalPipe
  ],
  templateUrl: './recipe-details.html',
  styleUrl: './recipe-details.scss',
})
export class RecipeDetails {

  @Input() recipe: Recipe | null = null;

  nutrition: NutritionInfo | null = null;
  isLoadingNutrition = false;
  errorMessage: string | null = null;

  constructor(private recipesService: RecipesService) {}

  loadNutrition(): void {
    if (!this.recipe) {
      return;
    }
    this.nutrition = null;
    this.errorMessage = null;
    this.isLoadingNutrition = true;

    this.recipesService.getRecipeNutritions(this.recipe.ID).subscribe({
      next: res => {
        if (res.Success) {
          this.nutrition = res.Object;
        } else {
          console.error(res.Message);
          this.errorMessage = 'Failed to load nutrition info.';
        }

        this.isLoadingNutrition = false;
      },
      error: err => {
        console.error(err);
        this.errorMessage = 'Failed to load nutrition info.';
        this.isLoadingNutrition = false;
      }
    });
  }
}
