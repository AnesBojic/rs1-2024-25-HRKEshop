import { NgModule } from '@angular/core';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import {MatButtonToggle, MatButtonToggleModule} from "@angular/material/button-toggle";
import {MatChipAvatar} from "@angular/material/chips";
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatListModule} from '@angular/material/list';
import {MatCardModule} from '@angular/material/card';
import {MatCheckboxModule} from '@angular/material/checkbox';




@NgModule({
  declarations: [],
  exports:[
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatProgressBarModule,
      MatButtonToggleModule,
    MatSidenavModule,
    MatListModule,
    MatCardModule,
    MatCheckboxModule

  ]
})
export class MaterialModule { }
