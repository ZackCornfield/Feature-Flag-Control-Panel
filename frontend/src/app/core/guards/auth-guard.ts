import { inject } from '@angular/core/primitives/di';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated() && !authService.isTokenExpired()) {
    console.log('Access granted to route');
    return true;
  }

  // Not authenticated, redirect to login
  router.navigate(['/control-panel/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
