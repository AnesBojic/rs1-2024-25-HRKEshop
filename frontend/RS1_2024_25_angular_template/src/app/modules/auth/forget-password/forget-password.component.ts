import {Component} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {MatSnackBar} from '@angular/material/snack-bar';
import {AuthApi} from '../../../api/auth.api';
import {ForgotPasswordDto} from '../../../dto/auth.dto';

@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
  styleUrl: './forget-password.component.css',
  standalone: false
})
export class ForgetPasswordComponent {

  protected readonly onsubmit = onsubmit;
  email:string='';
  message:string='';
  loading:boolean= false;

  constructor(private authApi:AuthApi,private snackBar:MatSnackBar) {
  }



  onSubmit()
  {
    if(!this.email) return;

    const dto: ForgotPasswordDto =
      {
        email:this.email
      }


    this.loading = true;
    this.authApi.forgotPassword(dto).subscribe({
      next:()=> {
        this.snackBar.open(
          'If email exists, a reset link has been sent.',
          'Close',
          {duration: 4000}
        );
        this.loading = false;
      },
      error:()=>
      {
        this.snackBar.open(
          'Something went wrong, but if email exists, a reset link has been sent.',
          'Close',
          {duration:4000}
        );
        this.loading = false;
      }
    })

  }
}
