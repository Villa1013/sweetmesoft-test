import { Routes } from '@angular/router';
import { Shell } from './layout/shell/shell';

export const routes: Routes = [
  {
    path: '',
    component: Shell,
    children: [
      { path: '', redirectTo: 'panel', pathMatch: 'full' },
      {
        path: 'panel',
        loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'solicitudes',
        loadComponent: () =>
          import('./features/solicitudes/solicitudes-list').then((m) => m.SolicitudesList),
      },
      {
        path: 'solicitudes/nueva',
        loadComponent: () =>
          import('./features/solicitudes/solicitud-nueva').then((m) => m.SolicitudNueva),
      },
      {
        path: 'solicitudes/:id',
        loadComponent: () =>
          import('./features/solicitudes/solicitud-detail').then((m) => m.SolicitudDetail),
      },
      {
        path: 'cotizaciones',
        loadComponent: () =>
          import('./features/cotizaciones/cotizaciones-list').then((m) => m.CotizacionesList),
      },
      {
        path: 'cotizaciones/:id',
        loadComponent: () =>
          import('./features/cotizaciones/cotizacion-detail').then((m) => m.CotizacionDetail),
      },
      {
        path: 'tarifas',
        loadComponent: () => import('./features/tarifas/tarifas').then((m) => m.Tarifas),
      },
    ],
  },
  { path: '**', redirectTo: 'panel' },
];
