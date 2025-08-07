import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClientRoutingModule } from './client-routing.module';
import { ReservationComponent } from './reservation/reservation.component';
import { UserProfileComponent } from './user-profile/user-profile.component';
import {MaterialModule} from '../material/material.module';
import {ReactiveFormsModule} from '@angular/forms';
import { ClientLayoutComponent } from './client-layout/client-layout.component';
import {SharedModule} from '../shared/shared.module';



@NgModule({
  declarations: [
    ReservationComponent,
    UserProfileComponent,
    ClientLayoutComponent,

  ],
  imports: [
    CommonModule,
    ClientRoutingModule,
    MaterialModule,
    ReactiveFormsModule,
    SharedModule
  ]
})
export class ClientModule { }
