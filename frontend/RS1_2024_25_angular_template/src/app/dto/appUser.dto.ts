export  interface AppuserUpdateRequestDto
{
  ID:number,
  name?:string,
  surname?:string,
  email?:string,
  phone?:string,
  address?:string,
  city?:string,
  zipcode?:string
}
export  interface  UserProfileResponse
{

  id: number;
  name:string;
  surname:string;
  email:string;
  address:string,
  city:string,
  zipCode:string,
  phone:string,
}
