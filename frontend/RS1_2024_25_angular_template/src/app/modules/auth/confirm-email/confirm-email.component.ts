import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {AuthApi} from '../../../api/auth.api';
import {ConfirmEmailRequestDto} from '../../../dto/auth.dto';

@Component({
  selector: 'app-confirm-email',
  standalone: false,
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.css'
})
export class ConfirmEmailComponent implements  OnInit{

  message:string ="Processing...";


  constructor(private  route:ActivatedRoute,private authApi:AuthApi,private router:Router) {
  }

  ngOnInit(): void {

        const urlToken =this.route.snapshot.queryParamMap.get('token');

        if(!urlToken)
        {
          this.message = "Invalid confirmation link..";
          return;
        }

        const token = decodeURIComponent(urlToken);


        if(!token)
        {
          this.message = "Invalid confirmation link..";
          return;

        }

        const dto : ConfirmEmailRequestDto = {
          token:token
        };

        this.authApi.confirmEmail(dto).subscribe({
          next:(res)=>
          {

            this.message = "✅ Your email has been verified!";
            this.router.navigate(["auth/login"])

          },
          error:(err)=>
          {
            this.message = "❌ Verification failed or token expired.";
          }

        })


    }






}
