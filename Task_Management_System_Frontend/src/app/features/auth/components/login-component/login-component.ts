import { Component, inject, OnInit, signal, ViewEncapsulation } from '@angular/core';
import { Form, FormBuilder, FormGroup, ReactiveFormsModule, Validators, ɵInternalFormsSharedModule } from '@angular/forms';
import { Router } from '@angular/router';
import {  ButtonModule } from "primeng/button";
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { PanelModule } from 'primeng/panel';
import { PasswordModule } from 'primeng/password';
import { AuthService } from '../../../../core/services/auth-service';
import { LoginModel } from '../../../../core/models/login-model';
import { ApiResponseModel } from '../../../../core/models/api-response-model';
import { ExecutionResultEnum } from '../../../../core/constants';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-login-component',
  imports: [ButtonModule, PanelModule, InputTextModule, CheckboxModule, PasswordModule,
    ReactiveFormsModule, ToastModule
  ],
  templateUrl: './login-component.html',
  styleUrl: './login-component.css',
  encapsulation: ViewEncapsulation.None,
  providers:[MessageService]
})

export class LoginComponent implements OnInit {

  authService = inject(AuthService);
  messageService = inject(MessageService);
  formBuilder = inject(FormBuilder);
  router = inject(Router);

 
  loginForm:FormGroup;

  isLoading = signal(false);

  constructor(){
    this.loginForm = this.formBuilder.group({
      username: [null, Validators.required],
      password: [null, Validators.required],
      rememberMe: [false]
    })
  }

  ngOnInit(): void {
    this.authService.logout();
  }


  onLogin():void{
    if(this.loginForm.valid){

      this.isLoading.set(true);

      const obj:LoginModel = this.loginForm.value;

      this.authService.login(obj).subscribe({
        next:(response:ApiResponseModel) =>{
          this.router.navigateByUrl("/task");
          this.isLoading.set(false);
        },
        error:(err)=>{
          this.isLoading.set(false);
          this.messageService.add({severity:'error', summary: 'Error', detail: err.error?.responseText ?? 'An error occured while login.'});
        }
      })

    } else {
      this.loginForm.markAllAsDirty();
      this.loginForm.markAllAsTouched();
    }
  }
}
