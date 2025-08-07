export  interface ImageGetByEntityRequest
{
  ImageableId:number,
  ImageableType:string
}
export interface  ImageGetByEntityResponse
{
  id:number,
  name?:string
  url:string
}
export  interface  ImageUpdateResponse
{
  id:number,
  message:string
}
export interface  ImageUploadResponse
{
  imageId:number,
  url:string
}
