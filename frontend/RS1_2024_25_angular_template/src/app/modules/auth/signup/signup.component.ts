import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

import {Router} from '@angular/router';
import {SignupRequestDto} from '../../../dto/auth.dto';
import {MySnackbarHelperService} from '../../shared/snackbars/my-snackbar-helper.service';
import {AuthApi} from '../../../api/auth.api';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-signup',
  standalone: false,
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent implements  OnInit{

  signupForm!:FormGroup;
  passwordStregth: number = 0;

  loading:boolean = false;

  constructor(private  fb:FormBuilder,private  authApi:AuthApi,private  router:Router,private  snackBar:MatSnackBar) {
  }

  ngOnInit() :void{
    this.signupForm = this.fb.group({
      firstName:['',[Validators.required]],
      lastName:['',[Validators.required]],
      email:['',[Validators.required,Validators.email]],
      password:['',[Validators.required,Validators.minLength(6)]],
      confirmPassword:['',Validators.required],
      phone:['']
    });

    this.signupForm.valueChanges.subscribe(()=>
    {
      this.checkPasswords()
    })
  }

  checkPasswords() {
    const password = this.signupForm.get('password')?.value;
    const confirmPassword = this.signupForm.get('confirmPassword');

    if (!confirmPassword) return;

    if (password !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      // Clear only passwordMismatch error if passwords match
      if (confirmPassword.hasError('passwordMismatch')) {
        confirmPassword.setErrors(null);
      }
    }
  }


  onSubmit(): void
  {
    if(this.signupForm.invalid)
    {
      this.signupForm.markAllAsTouched();
      return;
    }

    const dto: SignupRequestDto = {
      name :this.signupForm.value.firstName,
      surname:this.signupForm.value.lastName,
      email:this.signupForm.value.email,
      password:this.signupForm.value.password,
      phone:this.signupForm.value.phone
    }
    this.loading = true;

    this.authApi.signup(dto).subscribe({
      next:(response)=>
      {
        this.loading = false;
        this.snackBar.open("Signup successfull, email verification needed, proceed to confirm your email.",'close',{duration:3000,horizontalPosition:"center",verticalPosition:"top"})
        this.router.navigate(['/auth/resend-email'],{queryParams:{email:dto.email,name:dto.name}})

      },
      error: (err)=>
      {
        this.loading = false;
        this.snackBar.open("Signup failed",'close',{duration:3000});
      }
    })


  }

  onPasswordInput()
  {
      const password = this.signupForm.get('password')?.value || '';
      this.passwordStregth = this.EvaluatePasswordStrength(password);
  }

  getStrengthColor(pwstrentgh:number) : 'primary' | 'accent' | 'warn'
  {
    if(pwstrentgh <=2) return 'warn';
    if(pwstrentgh <=4) return  'accent';
    return  'primary'
  }
  getStrengthLabel(pwstrength:number) :string
  {
    if(pwstrength <=2) return 'What is this man ?';
    if(pwstrength <=4) return  'Okay, this is better';
    return  'Okay mate its alright,i get it.'
  }
  getStrengthClass(pwtstrength:number) : string
{
    if(pwtstrength <=2) return "weak-strength";
    if(pwtstrength <=4) return "fair-strength"
    return  "strong-strength"

}


  EvaluatePasswordStrength(password:string): number
  {
    let score = 0;
    if(!password) return  score;


    if(password.length > 6) score++;
    if(password.length > 12) score++;


    if(/[a-z]/.test(password)) score++;

    if(/[A-Z]/.test(password)) score++;

    if(/\d/.test(password)) score++;

    if(/[^A-Za-z0-9]/.test(password)) score++;

    return  score;
  }




}
