// src/app/validators/must-match.validator.ts
import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';

export function MustMatch(controlName: string, matchingControlName: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const formGroup = control as FormGroup;
    const controlToMatch = formGroup.controls[controlName];
    const matchingControl = formGroup.controls[matchingControlName];

    if (!controlToMatch || !matchingControl) {
      // One of the controls is not found in the FormGroup
      return null; // or return { controlNotFound: true } to indicate the error
    }

    // Checking if the matchingControl has any errors other than mustMatch
    if (matchingControl.errors && !matchingControl.errors['mustMatch']) {
      return null;
    }

    // Set error on matchingControl if validation fails
    if (controlToMatch.value !== matchingControl.value) {
      matchingControl.setErrors({ mustMatch: true });
      return { mustMatch: true };
    } else {
      matchingControl.setErrors(null);
    }
    return null;
  };
}
