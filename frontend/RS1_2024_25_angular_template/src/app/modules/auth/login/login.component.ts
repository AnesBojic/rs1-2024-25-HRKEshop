import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {AuthApi} from '../../../api/auth.api';
import {LoginRequestDto} from '../../../dto/auth.dto';
import {AuthService} from '../../../services/auth-services/auth.service';
import {Router} from '@angular/router';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: false,
})
export class LoginComponent implements  OnInit{

  form!:FormGroup;
  hidePassword=true;
  errorHandler:string | null = null;

  constructor(private fb: FormBuilder,private authApi:AuthApi,private authService:AuthService,private router:Router) {
  }

  ngOnInit() :void {

    this.form = this.fb.group({
      email:['',[Validators.required,Validators.email]],
      password:['',Validators.required]
    });
    this.form.valueChanges.subscribe(()=>{
      this.errorHandler = null;
    })
  }

  onSubmit() : void
  {

    if(this.form.invalid) return;

    const {email,password} =this.form.value;

    const loginRequest:LoginRequestDto = {email,password};






    this.authApi.login(loginRequest).subscribe({
      next:response=>{

        console.log('Login succesfull',response);
        this.authService.setTokens(response.token,response.refreshToken);
        this.router.navigate(['/public']);

      },
      error:err => {

        console.error("Login failed",err);

        if(err.error && err.error.errorCode && err.error.message)
        {
          this.errorHandler = err.error.message;
        }
        else
        {
          this.errorHandler = "An unexecpted error occurred. Please try again later.";
        }

        setTimeout(()=>{
          this.errorHandler = null;
        },5000)

      }
    })
  }



}
