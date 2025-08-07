import {Component, OnInit, ViewChild} from '@angular/core';
import {MyPagedList} from '../../../helper/my-paged-list';
import {MyPagedRequest} from '../../../helper/my-paged-request';
import {MyConfig} from '../../../my-config';
import {HttpClient, HttpParams} from '@angular/common/http';
import {buildHttpParams} from '../../../helper/http-params.helper';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';
import {Subject,of} from 'rxjs';
import  {debounceTime,distinctUntilChanged,switchMap,catchError,tap} from 'rxjs/operators';
import {getMatFormFieldDuplicatedHintError} from '@angular/material/form-field';


export  interface  AppUser
{
  id:number,
  name:string,
  surname:string,
  email: string,
  phone: string,
  roleName: string,
  isLocked:boolean
}

export  interface AppUserGetAllRequest extends  MyPagedRequest
{
  q?:string;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  standalone: false
})
export class DashboardComponent implements  OnInit{

  apiUrl  = `${MyConfig.api_address}/appusers/filter`;
  pagedUsers : MyPagedList<AppUser> | null = null;
  request : AppUserGetAllRequest = {
    pageNumber : 1,
    pageSize : 10,
    q:''
  };
  loading = false;
  error:string | null = null;

  searchTerms = new Subject<string>();






  constructor(private http:HttpClient) {
  }

  ngOnInit() {
    this.fetchUsers();
  }

  fetchUsers() : void {

    this.loading =true;
    this.error = null;


    const params = buildHttpParams(this.request);

    this.http.get<MyPagedList<AppUser>>(`${this.apiUrl}`,{params})
      .subscribe(
        {
          next:(res)=>
          {
            setTimeout(() => {
              this.pagedUsers = res;
              this.loading = false;
            }, 200);

          },
          error:(err)=>
          {
            console.error("Failed to fetch users" ,err);
            this.error = "Failed to fetch users";
            this.loading = false;
          }
        }
      )

  }

  onSearchChange(): void {
    this.request.pageNumber = 1;
    this.fetchUsers();
  }
  onPageChange(event: PageEvent): void {
    this.request.pageNumber = event.pageIndex + 1;
    this.request.pageSize = event.pageSize;
    this.fetchUsers();
  }




}
