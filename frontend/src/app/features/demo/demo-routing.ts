import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';

export const DEMO_ROUTES: Routes = [
  {
    path: '',
    component: Dashboard,
  },
];
