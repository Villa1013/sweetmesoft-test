import { registerLocaleData } from '@angular/common';
import localeEsCo from '@angular/common/locales/es-CO';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { LOCALE_ID, ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import Aura from '@primeng/themes/aura';
import { definePreset } from '@primeng/themes';
import { providePrimeNG } from 'primeng/config';

import { routes } from './app.routes';

registerLocaleData(localeEsCo, 'es-CO');

const AppPreset = definePreset(Aura, {
  components: {
    tag: {
      root: {
        fontSize: '0.75rem',
        padding: '0.125rem 0.5rem',
        gap: '0.25rem',
      },
      icon: { size: '0.625rem' },
    },
  },
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch()),
    provideAnimationsAsync(),
    { provide: LOCALE_ID, useValue: 'es-CO' },
    providePrimeNG({
      ripple: true,
      theme: {
        preset: AppPreset,
        options: {
          darkModeSelector: '.p-dark',
          components: { 
            tag: { 
              root: { 
                fontSize: '0.75rem', padding: '0.125rem 0.5rem', gap: '0.25rem' 
              }, 
              icon: { 
                size: '0.625rem' 
              } 
            } 
          },
        },
      },
      translation: {
        firstDayOfWeek: 1,
        dayNames: ['domingo', 'lunes', 'martes', 'miércoles', 'jueves', 'viernes', 'sábado'],
        dayNamesShort: ['dom', 'lun', 'mar', 'mié', 'jue', 'vie', 'sáb'],
        dayNamesMin: ['D', 'L', 'M', 'X', 'J', 'V', 'S'],
        monthNames: [
          'enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio',
          'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre',
        ],
        monthNamesShort: [
          'ene', 'feb', 'mar', 'abr', 'may', 'jun',
          'jul', 'ago', 'sep', 'oct', 'nov', 'dic',
        ],
        today: 'Hoy',
        clear: 'Limpiar',
        weekHeader: 'Sem',
        dateFormat: 'dd/mm/yy',
      },
    }),
  ],
};
