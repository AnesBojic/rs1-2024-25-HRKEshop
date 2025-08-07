import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AbstractControl, FormBuilder, FormGroup, ValidationErrors} from '@angular/forms';
import {Validators} from '@angular/forms';
import {ResetPasswordRequestDto, ResetPasswordResponseDto} from '../../../dto/auth.dto';
import {AuthApi} from '../../../api/auth.api';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-reset-password',
  standalone: false,
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements  OnInit{

  token:string = '';
  resetForm!: FormGroup;

  constructor(private route: ActivatedRoute,private fb:FormBuilder,private authApi:AuthApi,private snekBar:MatSnackBar,private router:Router) {
  }

    ngOnInit(): void {
        this.route.queryParams.subscribe(params=>
        {
          this.token = params['token'];
        })

      this.resetForm = this.fb.group({
          newPassword:['',[Validators.required,Validators.minLength(6)]],
          confirmPassword:['',Validators.required]
        }, {
        validators:this.passwordMatchValidator
      });

    }

  passwordMatchValidator(form: AbstractControl): ValidationErrors | null {
    const password = form.get('newPassword')?.value;
    const confirm = form.get('confirmPassword')?.value;
    return password === confirm ? null : { mismatch: true };
  }

    get newPassword()
    {
      return this.resetForm.get('newPassword');
    }
    get confirmPassword()
    {
      return this.resetForm.get('confirmPassword');
    }


  onSubmit() {
    if(this.resetForm.invalid) return;

    const dto : ResetPasswordRequestDto =
    {
      token:this.token,
      newPassword:this.confirmPassword?.value
    }

    console.log(dto);
    this.authApi.resetPassword(dto).subscribe({
      next:(res:ResetPasswordResponseDto)=>{
          this.snekBar.open(res.message,'Close',{
            duration:3000,
            horizontalPosition:'center',
            verticalPosition:'top'
          })

        this.router.navigate(['/auth/login'])
      },
      error:(err)=>{
        this.snekBar.open('Failed to reset password','Close',{
          duration:3000,
          horizontalPosition:'center',
          verticalPosition:'top'
        })
        console.log(err);
      }
    })




  }
}
