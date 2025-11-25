import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private apiUrl = 'http://localhost:7000/api/chat'; // adjust to your backend route

  constructor(private http: HttpClient) { }

  sendMessage(message: string): Observable<{ response: string }> {
    return this.http.post<{ response: string }>(`${this.apiUrl}/send`, { message });
  }
}
