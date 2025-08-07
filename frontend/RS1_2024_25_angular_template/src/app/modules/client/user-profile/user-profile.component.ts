import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {UserProfileResponse,AppuserUpdateRequestDto} from '../../../dto/appUser.dto';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ImageGetByEntityResponse,ImageGetByEntityRequest} from '../../../dto/image.dto';
import {MyConfig} from '../../../my-config';
import {AppUserApi} from '../../../api/appUser.api';
import {ImageApi} from '../../../api/image.api';

@Component({
  selector: 'app-user-profile',
  standalone: false,
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css'
})
export class UserProfileComponent implements  OnInit{

  userProfile?:UserProfileResponse;
  imageInfo? :ImageGetByEntityResponse;
  profileForm!:FormGroup;
  loading=false;
  errorMessage = '';
  isEditing = false;
  @ViewChild('imageInput') imageInput!:ElementRef<HTMLInputElement>;

  constructor(private userApi:AppUserApi,private fb:FormBuilder,private imageApi:ImageApi) {
  }

  triggerImageUpload():void {

      this.imageInput.nativeElement.click()

  }

  onImageSelected(event: Event) : void
  {


    const file = (event.target as HTMLInputElement)?.files?.[0];
    if(!file) return;

    if(!file.type.startsWith('image/')){
      alert('Please select valid image.')
      return;
    }

    if(this.imageInfo && this.imageInfo.id === 0)
    {

        this.uploadNewImage(file);

    }
    else if(this.imageInfo && this.imageInfo.id > 0)
    {

      this.updateImage(file);

    }

    const reader = new FileReader();
    reader.onload = () => {
      this.imageInfo!.url = reader.result as string;
    }
    reader.readAsDataURL(file);

  }


  updateImage(file:File)
  {
    const formData = new FormData();
    formData.append('Id',this.imageInfo!.id.toString());
    formData.append('ImageableId',this.userProfile!.id.toString());
    formData.append('Imageabletype','users');
    formData.append('File',file);

    this.imageApi.imageUpdate(formData).subscribe({
      next:(res)=>
      {

        this.loadImageProfile();
      },
      error:(err)=>
      {
        console.log('Error while uploading image',err);
      }
    })



  }

  uploadNewImage(file:File)
  {
    const formData = new FormData();
    formData.append('Name',`Profile image ${this.userProfile?.name+""+this.userProfile?.surname}`);
    formData.append('ImageableId',this.userProfile!.id.toString());
    formData.append('Imageabletype','users');
    formData.append('File',file);


    this.imageApi.imageUpload(formData).subscribe({
      next:(res)=>
      {
        console.log("Upload good");
        this.imageInfo = {
          id: res.imageId,
          url : res.url
        }
        this.loadImageProfile();
      },
      error:(err)=>
      {
        console.log('Error while uploading image',err);
      }
    })



  }


  ngOnInit(): void {

    this.loadUserProfile();

  }

  loadUserProfile() : void
  {
    this.loading = true;
    this.userApi.getUserProfile().subscribe({
      next:(profile)=>{
        this.userProfile = profile;
        this.loading =false;
        this.initForm(profile);
        this.loadImageProfile();
        console.log('Current image URL:', this.imageInfo?.url);

      },
      error:(error)=>
      {
        this.errorMessage = "Failed to load user profile.";
        console.error(error)
        this.loading = false;
      }
    })
  }

  initForm(profile:UserProfileResponse): void
  {
    this.profileForm = this.fb.group({
      name:[profile.name,[Validators.required,Validators.minLength(2)]],
      surname:[profile.surname,[Validators.required,Validators.minLength(2)]],
      email:[profile.email,[Validators.required,Validators.email]],
      city:[profile.city,],
      address:[profile.address],
      zipCode:[profile.zipCode],
      phone:[profile.phone]
    });
  }

  startEdit() : void
  {
    this.isEditing = true;
  }
  cancelEdit() : void
  {
    this.isEditing = false;
    this.profileForm.reset(this.userProfile);
  }

  saveEdit() :void
  {
      if(this.profileForm.invalid && !this.userProfile)
      {
        this.profileForm.markAllAsTouched();
        return;
      }

      const request : AppuserUpdateRequestDto =
        {
          ID:this.userProfile!.id,
          ...this.profileForm.value
        }

      this.userApi.updateUserProfile(request).subscribe({
        next:()=>
        {
          this.isEditing =false;
          this.loadUserProfile();
          console.log("Profile updated succesfully");
        },
        error:(err)=>
        {
          console.error("Failed to update profile",err);
          this.errorMessage = "Failed to update profile"
        }
      })
  }

  loadImageProfile() :void
  {
    if(!this.userProfile)
    {
      this.imageInfo = {
        id:0,
        url:'/images/defaulAvatar.jpg'
      };
      return;
    }


    const request:ImageGetByEntityRequest =
      {
        ImageableId: this.userProfile.id,
        ImageableType: 'users'
      };


    this.imageApi.getImageByEntity(request).subscribe({
      next:(response:ImageGetByEntityResponse[])=>
      {
        if(response.length > 0 && response[0].url)
        {
            this.imageInfo = {
              ...response[0],
              url:`${MyConfig.api_address}${response[0].url}`
            };
        }
        else
        {
          this.imageInfo = {
            id:0,
            url:'/images/defaulAvatar.jpg'
          }
        }


      },
      error: (err)=>
      {
        console.error("Failed to fetch image",err);
        this.imageInfo = {
          id:0,
          url:'/images/defaulAvatar.jpg'
        }
      }
    });

  }

  onImageError(event:Event)
  {
    const imgElemenet = event.target as HTMLImageElement;
    imgElemenet.src = '/images/defaulAvatar.jpg';
  }





}
