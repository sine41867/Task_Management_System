import { Component, inject, signal } from '@angular/core';
import { MenubarModule } from 'primeng/menubar';
import { AuthService } from '../../../core/services/auth-service';
import { Router } from '@angular/router';
import { SplitButtonModule } from 'primeng/splitbutton';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-top-nav-bar-component',
  imports: [MenubarModule, SplitButtonModule],
  templateUrl: './top-nav-bar-component.html',
  styleUrl: './top-nav-bar-component.css',
})
export class TopNavBarComponent {

  authService = inject(AuthService);
  router = inject(Router);

  firstName = signal('');

  items:MenuItem[] = [
    {
      label: 'Logout',
      command: () => {
          this.logout();
      }
    },
  ]

  constructor(){
    this.firstName.set(this.authService.getUserFirstName())
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

}
