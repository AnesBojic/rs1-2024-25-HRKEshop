import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router} from '@angular/router';
import {AuthService} from '../services/auth-services/auth.service';

export class AuthGuardData {
  isAdmin?: boolean;
  isManager?: boolean;
  isCustomer?:boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {
  }

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const guardData = route.data as AuthGuardData;  // Cast to AuthGuardData



    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return false;
    }

    // Provjera prava pristupa za administratora
    if (guardData.isAdmin && this.authService.isAdmin()) {
      return true;
    }

    // Provjera prava pristupa za menadžera
    if (guardData.isManager && this.authService.isManager()) {
      return true;
    }
    //Provjera prava pristupa za customera
    if(guardData.isCustomer && this.authService.isCustomer())
    {
      return true;
    }

    if(!guardData.isCustomer && !guardData.isAdmin && !guardData.isCustomer)
    {
      return true;
    }

    this.router.navigate(['/unauthorized']);
    return false;
  }

}
