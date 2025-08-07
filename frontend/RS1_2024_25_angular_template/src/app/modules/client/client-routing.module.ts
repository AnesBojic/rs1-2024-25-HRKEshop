import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {ReservationComponent} from './reservation/reservation.component';
import {UserProfileComponent} from './user-profile/user-profile.component';
import {AuthGuard} from '../../auth-guards/auth-guard.service';
import {ClientLayoutComponent} from './client-layout/client-layout.component';

const routes: Routes = [


  {
  path:'',component:ClientLayoutComponent,children: [
  {path: 'reservation', component: ReservationComponent},
  {path:'user-profile', component:UserProfileComponent, canActivate:[AuthGuard]}]
}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ClientRoutingModule {
}
