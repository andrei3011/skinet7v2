import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, switchMap, take } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  token?: string;

  constructor(private accountService: AccountService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // debugger
    // this.accountService.currentUser$.pipe(take(1)).subscribe({
    //   next: user => this.token = user?.token
    // });

    // if (this.token) {
    //   request = request.clone({
    //     setHeaders: {
    //       Authorization: `Bearer ${this.token}`
    //     }
    //   });
    // }

    // return next.handle(request);

    if (request.url.includes('account')
      && (!['emailExists', 'address'].some(u => request.url.includes(u))))
      return next.handle(request);

    return this.accountService.currentUser$.pipe(
      take(1),
      switchMap(user => {
        this.token = user?.token;
        if (this.token) {
          request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${this.token}`
            }
          });
        }
        return next.handle(request);
      })
    );
  }
}
