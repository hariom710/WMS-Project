import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

// Make sure this path matches where you saved your interceptor file!
import { authInterceptor } from './auth/auth.interceptor'; 

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes), 
    provideAnimationsAsync(),
    
    // THIS IS THE CRUCIAL LINE: It turns on API calls AND attaches your security token!
    provideHttpClient(withInterceptors([authInterceptor])) 
  ]
};