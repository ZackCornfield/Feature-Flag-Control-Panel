import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'control-panel',
    loadChildren: () =>
      import('./features/control-panel/control-panel-routing').then((m) => m.CONTROL_PANEL_ROUTES),
  },
  {
    path: 'demo',
    loadChildren: () => import('./features/demo/demo-routing').then((m) => m.DEMO_ROUTES),
  },
  {
    path: '',
    redirectTo: 'demo',
    pathMatch: 'full',
  },
];
