import {Component, Input, OnInit} from '@angular/core';
import {AuthService} from '../../../services/auth-services/auth.service';
import {Router} from '@angular/router';
import {MyConfig} from '../../../my-config';
import {ImageGetByEntityRequest} from '../../../dto/image.dto';
import {NAV_LINKS, NavLink, ProfileAction} from '../../../helper/NavLink';
import {ImageApi} from '../../../api/image.api';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements  OnInit{

  @Input() toggleSideNav!:()=> void;
  isLogged:boolean = false;
  imageInfo:string = '/images/defaulAvatar.jpg'
  isAdmin = false;
  isManager = false;
  name:string = '';

  profileActions:ProfileAction[] = [];

  constructor(private authService:AuthService,private  router:Router,private imageApiservice:ImageApi) {
  }

  ngOnInit(): void {

          this.authService.loggedIn$.subscribe(status=>
          {
            this.isLogged = status;
            this.profileActions = [];

            if (status)
            {
              this.loadImageAvatar();
              this.getNameOfUser();
              this.isAdmin = this.authService.isAdmin();
              this.isManager = this.authService.isManager();

              if(this.isAdmin)
              {
                this.profileActions.push({
                  label:'Admin panel',
                  action:()=> this.goToAdminPanel()
                });
              }
              this.profileActions.push(
                {
                  label:'Edit profile',
                  action:()=>this.goToProfile()
                }
              )

              this.profileActions.push({
                label:'Logout',
                action:()=>this.logout()
              })


            }
            else
            {
              this.imageInfo = 'images/defaulAvatar.jpg';
              this.name = '';
              this.isAdmin = false;
              this.isManager = false;
            }

          })



    }

    onImageError(event :Event)
    {
      const inputImg = event.target as HTMLImageElement;
      inputImg.src = `/images/defaulAvatar.jpg`;
    }

  goToProfile(): void
  {

    this.router.navigate(['/client/user-profile']);

  }
  logout() : void
  {
      this.profileActions = [];
      this.router.navigate(['/auth/logout']);
  }


  loadImageAvatar()
  {
    if(this.isLogged)
    {

      var request:ImageGetByEntityRequest =
          {
            ImageableId : Number(this.authService.getUserId()),
            ImageableType : "users"
          }
        this.imageApiservice.getImageByEntity(request).subscribe({
          next: (response)=>
          {
            if(response.length > 0 && response[0].url)
            {
              this.imageInfo = `${MyConfig.api_address}${response[0].url}`;

            }
          },
          error:(err)=>
          {
            console.log("Error while fetching image for avatar",err);
          }
        })
    }
  }
  getNameOfUser()
  {
    this.name = this.authService.getName() ?? '';
  }


  goToAdminPanel() {
    this.router.navigate(['/admin'])
  }

  protected readonly NAV_LINKS = NAV_LINKS;
}
