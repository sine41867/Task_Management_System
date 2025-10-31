import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth-service';
import { tap } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const router = inject(Router);
  const authService = inject(AuthService);

  var modifiedReq = req;

  const token = authService.getToken();

  if(token != null){
    modifiedReq = req.clone({
      setHeaders:{
        Authorization: `Bearer ${token}`
      }
    })
  }

  return next(modifiedReq).pipe(
    tap({
      error: (err) => {
        if(err.status === 401 || err.status === 403){
          authService.logout();
          router.navigate(['/login']);
        }
      }
    })
  )
};
