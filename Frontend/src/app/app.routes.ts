import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { 
    path: 'login', 
    loadComponent: () => import('./components/auth/login/login.component').then(m => m.LoginComponent)
  },
  { 
    path: 'register', 
    loadComponent: () => import('./components/auth/register/register.component').then(m => m.RegisterComponent)
  },
  { 
    path: 'patients', 
    loadComponent: () => import('./components/patients/patient-list/patient-list.component').then(m => m.PatientListComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'patients/create', 
    loadComponent: () => import('./components/patients/patient-create/patient-create.component').then(m => m.PatientCreateComponent),
    canActivate: [authGuard, roleGuard(['Admin', 'Receptionist'])]
  },
  { 
    path: 'patients/edit/:id', 
    loadComponent: () => import('./components/patients/patient-edit/patient-edit.component').then(m => m.PatientEditComponent),
    canActivate: [authGuard, roleGuard(['Admin', 'Receptionist'])]
  },
  { 
    path: 'patients/view/:id', 
    loadComponent: () => import('./components/patients/patient-view/patient-view.component').then(m => m.PatientViewComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'activity-log', 
    loadComponent: () => import('./components/activity-log/activity-log.component').then(m => m.ActivityLogComponent),
    canActivate: [authGuard, roleGuard(['Admin'])]
  },
  { path: '**', redirectTo: '/login' }
];
