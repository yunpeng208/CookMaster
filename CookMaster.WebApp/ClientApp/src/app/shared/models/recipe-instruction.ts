import {RecipeInstructionStep} from './recipe-instruction-step';

export interface RecipeInstruction{
  Name: string;
  Steps: RecipeInstructionStep[];
}
