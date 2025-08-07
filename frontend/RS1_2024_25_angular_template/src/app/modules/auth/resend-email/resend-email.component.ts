import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {MatSnackBar} from '@angular/material/snack-bar';
import {AuthApi} from '../../../api/auth.api';
import {ResendEmailRequstDto} from '../../../dto/auth.dto';

@Component({
  selector: 'app-resend-email',
  standalone: false,
  templateUrl: './resend-email.component.html',
  styleUrl: './resend-email.component.css'
})
export class ResendEmailComponent implements  OnInit{


  constructor(private route:ActivatedRoute,private snek:MatSnackBar,private authApi:AuthApi) {
  }

  email:string  = "user";
  name:string ="user";

  ngOnInit(): void {
    this.email = this.route.snapshot.queryParamMap.get('email')!;
    this.name = this.route.snapshot.queryParamMap.get('name')!;
  }



  onClickResendMailAndSnackbar() :void
  {


    const dto: ResendEmailRequstDto = {
          email:this.email!
    }

    console.log(dto);

    this.authApi.resendEmail(dto).subscribe({
      next:(res)=>{
        this.snek.open("Mail has been sent. Check your inbox",'Close',{duration:3000,horizontalPosition:"center",verticalPosition:"top"})
      },
      error:(err)=>
      {
        console.error(err,"Something went wrong!");
      }
    })

  }




}
