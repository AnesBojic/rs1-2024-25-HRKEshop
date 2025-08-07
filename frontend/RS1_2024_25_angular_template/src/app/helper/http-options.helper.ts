import  {HttpHeaders} from '@angular/common/http';


export  function httpOptionsHelper() : {headers:HttpHeaders}{
  return {
    headers: new HttpHeaders({
      'X-Skip-Global-Error':'true'
    })
  };
}
