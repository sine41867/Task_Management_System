import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
    {
    path: '',
    loadChildren: () =>
      import('./features/auth/auth-module').then(m => m.AuthModule),
    },

    {
        path: 'task',
        canActivate: [authGuard],
        loadChildren: () =>
          import('./features/task/task-module').then(m => m.TaskModule),
    },
    {
        path: '**',
        redirectTo: ''
    }
];
