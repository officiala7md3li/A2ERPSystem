import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  // Get token from localStorage
  const token = localStorage.getItem('auth_token');
  
  // If token exists, clone the request and add the authorization header
  if (token) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(authReq);
  }
  
  // Otherwise, proceed with the original request
  return next(req);
};