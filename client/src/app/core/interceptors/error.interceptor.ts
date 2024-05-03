import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (!error) return throwError(() => new Error("Unexpected error!"));

        switch (error.status) {
          case 400:
            if (error.error.errors && error.error.errors.length > 0)
              throw error.error;
            this.showErrorMessage(error);
            break;
          case 401:
            this.showErrorMessage(error);
            break;
          case 404:
            this.router.navigateByUrl('/not-found');
            break;
          case 500:
            const navigationExtras: NavigationExtras = { state: { error: error.error } };
            this.router.navigateByUrl('/server-error', navigationExtras);
            break;
          default:
            this.showErrorMessage(error);
            break;
        }

        return throwError(() => new Error(error.message));
      })
    );
  }

  showErrorMessage(error: HttpErrorResponse) {
    this.toastr.error(error.error.message, error.status.toString());
  }
}
