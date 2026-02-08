import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FeatureFlagList } from './pages/feature-flag-list/feature-flag-list';
import { FeatureFlagEdit } from './pages/feature-flag-edit/feature-flag-edit';
import { FeatureFlagOverrides } from './pages/feature-flag-overrides/feature-flag-overrides';
import { Register } from './pages/auth/register/register';
import { Login } from './pages/auth/login/login';
import { authGuard } from '../../core/guards/auth-guard';
import { authInterceptor } from '../../core/interceptors/auth-interceptor';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

export const CONTROL_PANEL_ROUTES: Routes = [
  {
    path: '',
    children: [
      {
        path: '',
        canActivate: [authGuard],
        component: FeatureFlagList,
      },
      {
        path: 'create',
        canActivate: [authGuard],
        component: FeatureFlagEdit,
      },
      {
        path: ':id/edit',
        canActivate: [authGuard],
        component: FeatureFlagEdit,
      },
      {
        path: 'overrides',
        canActivate: [authGuard],
        component: FeatureFlagOverrides,
      },
      {
        path: 'login',
        component: Login,
      },
      {
        path: 'register',
        component: Register,
      },
    ],
  },
];
