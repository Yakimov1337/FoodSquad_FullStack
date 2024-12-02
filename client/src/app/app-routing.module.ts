import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {isAllowedRoleGuard, isAuthenticatedGuard, isUnauthenticatedGuard } from './core/guards/role.guard';

const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./modules/auth/auth.module').then(m => m.AuthModule),
    canActivate: [isUnauthenticatedGuard], // Guard applied here to manage access to /auth based on authentication status and role
  },
  {
    path: 'admin',
    loadChildren: () => import('./modules/admin/admin.module').then(m => m.AdminModule),
    canActivate: [isAuthenticatedGuard, isAllowedRoleGuard], // Guard also manages access to /admin based on the user role
  },
  {
    path: '',
    loadChildren: () => import('./modules/user/user.module').then(m => m.UserModule),
    canActivate: [isAuthenticatedGuard],
  },
  {
    path: '**',
    redirectTo: 'error/404'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
