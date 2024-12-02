import { AbstractControl, ValidatorFn } from '@angular/forms';

export function urlFormValidator(): ValidatorFn {
  return (control: AbstractControl): {[key: string]: any} | null => {
    const value = control.value;

    // If value is empty, return an error indicating the URL is invalid.
    // This makes the validator not pass for empty strings.
    if (!value || !value.trim()) {
      return { invalidUrl: 'URL is required' };
    }

    try {
      new URL(value);
      return null; // The URL is valid
    } catch {
      return { invalidUrl: 'Invalid URL' }; // The URL is invalid
    }
  };
}


// FOR REACT FORMS USE
export function isValidUrl(url: URL): boolean {
  try {
    // Attempt to use the URL for any operation that would fail if invalid
    return Boolean(url.href);
  } catch {
    return false; // The URL is invalid
  }
}
