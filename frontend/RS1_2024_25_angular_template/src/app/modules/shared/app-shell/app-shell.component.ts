import {Component, OnInit} from '@angular/core';
import {AuthService} from '../../../services/auth-services/auth.service';
import {NAV_LINKS, ProfileAction} from '../../../helper/NavLink';
import {Router} from '@angular/router';

@Component({
  selector: 'app-app-shell',
  standalone: false,
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.css'
})
export class AppShellComponent implements OnInit{
  profileActions: ProfileAction[] = [];
  protected readonly NAV_LINKS = NAV_LINKS;

  constructor(public authService :AuthService,private  router:Router) {
  }

  ngOnInit() {
    this.authService.loggedIn$.subscribe(loggedIn => {
      if (loggedIn) {
        this.profileActions = [];

        if (this.authService.isAdmin()) {
          this.profileActions.push({
            label: 'Admin panel',
            action: () => this.goToAdminPanel()
          });
        }

        this.profileActions.push(
          {
            label: 'Edit profile',
            action: () => this.goToProfile()
          },
          {
            label: 'Logout',
            action: () => this.logout()
          }
        );
      } else {
        this.profileActions = [];
      }
    });
  }

  goToAdminPanel() {
    this.router.navigate(['/admin']);
  }

  goToProfile() {
    this.router.navigate(['/client/user-profile']);
  }

  logout() {


    this.router.navigate(['/auth/logout']);
  }



}
