export interface  NavLink{
  label:string,
  route:string,
  showIfLoggedIn?:boolean,
  showIfLoggedOut?:boolean
}

export interface  ProfileAction{
  label:string,
  action:()=> void;
  showIfAdminOnly?:boolean;
}

export  const NAV_LINKS:NavLink[] =[
  {label:'Home',route:'/public/home',showIfLoggedIn:true,showIfLoggedOut:true}

]




