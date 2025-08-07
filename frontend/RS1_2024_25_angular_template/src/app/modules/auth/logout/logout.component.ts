import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {MyConfig} from '../../../my-config';
import {AuthService} from '../../../services/auth-services/auth.service';
import {AuthApi} from '../../../api/auth.api';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css'],
  standalone: false
})
export class LogoutComponent implements OnInit {
  constructor(
    private httpClient: HttpClient,
    private authService: AuthService,
    private  authApi:AuthApi,
    private router: Router
  ) {
  }

  ngOnInit(): void {
    this.logout();
  }

  logout(): void {
    this.authApi.logout().subscribe({
      next:()=> this.handleLogoutSuccessOrError(),
      error:(error)=>{
        console.error('Error during logout.',error);
        this.handleLogoutSuccessOrError();
      }
    })
  }

  // Metoda za zajedničko uklanjanje tokena i preusmjeravanje
  private handleLogoutSuccessOrError(): void {
    this.authService.clearTokens();
    setTimeout(() => {
      this.router.navigate(['/auth/login']); // Preusmjeravanje na login nakon 3 sekunde
    }, 3000);
  }
}
