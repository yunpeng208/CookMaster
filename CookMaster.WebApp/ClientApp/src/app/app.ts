import { Component, signal } from '@angular/core';
import {RecipesSearch} from './core/recipes-search/recipes-search';

@Component({
  selector: 'app-root',
  imports: [RecipesSearch],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('CookMaster');
}
