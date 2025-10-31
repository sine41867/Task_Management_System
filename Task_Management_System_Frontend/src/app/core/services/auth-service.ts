import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environments';
import { LoginModel } from '../models/login-model';
import { Observable, tap } from 'rxjs';
import { ApiResponseModel } from '../models/api-response-model';
import { HttpClient } from '@angular/common/http';
import { ExecutionResultEnum } from '../constants';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  
  private baseUrl:string = `${environment.apiUrl}/Auth`

  private tokenKey = 'jwtToken';

  http = inject(HttpClient);

  firstName = signal<string | null>('');

  login(loginData:LoginModel):Observable<ApiResponseModel>{
    return this.http.post<ApiResponseModel>(`${this.baseUrl}/Login`, loginData)
      .pipe(
        tap(response => {
          if(response.executionResultId == ExecutionResultEnum.Success){
            localStorage.setItem(this.tokenKey, response.data.token);
            localStorage.setItem("FirstName", response.data.firstName);
            this.firstName.set(response.data.firstName);
          }
        })
      )
  }

  logout() : void {
    localStorage.removeItem(this.tokenKey);
    this.firstName.set('');
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return this.getToken() !== null;
  }

  getUserFirstName():string{
    return this.firstName() ?? '';
  }

  constructor(){
    if(this.isLoggedIn()){
      this.firstName.set(localStorage.getItem("FirstName"));
    }
  }

}
