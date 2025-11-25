import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipesSearch } from './recipes-search';

describe('RecipesSearch', () => {
  let component: RecipesSearch;
  let fixture: ComponentFixture<RecipesSearch>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecipesSearch]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecipesSearch);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
