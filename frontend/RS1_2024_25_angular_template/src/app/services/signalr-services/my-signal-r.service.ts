import {Injectable} from '@angular/core';
import * as signalR from '@microsoft/signalr';
import {MyConfig} from '../../my-config';
import {LoginTokenDto} from '../auth-services/dto/login-token-dto';

@Injectable({providedIn: 'root'})
export class MySignalRService {
  private hubConnection!: signalR.HubConnection;

  // Pokretanje SignalR konekcije
  startConnection() {
    let loginTokenDto: LoginTokenDto = JSON.parse(localStorage.getItem('my-auth-token') || '{}');
    const authToken = loginTokenDto.token;

    if (!authToken) {
      console.error('No auth token found, SignalR connection not started.');
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${MyConfig.api_address}/mysginalr-hub-path?my-auth-token=${authToken}`)
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch((err) => console.error('Error while connecting SignalR:', err));
  }

  // Zaustavljanje SignalR konekcije
  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection
        .stop()
        .then(() => console.log('SignalR connection stopped'))
        .catch((err) => console.error('Error while stopping SignalR connection:', err));
    }
  }

  // Adding listenera for reciving messages
  myClientMethod1(callback: (message: string) => void) {
    this.hubConnection.on('myClientMethod1', (data: string) => {
      console.log('Message received:', data);
      callback(data);
    });
  }

  // Sending messagess to server
  myServerHubMethod1(toUser: string, message: string) {
    this.hubConnection
      .invoke('MyServerHubMethod1', toUser, message)
      .then(() => console.log('Message sent successfully'))
      .catch((err) => console.error('Error while sending message:', err));
  }

  //Methods for ChatBot
  sendTyping(toUser: string) {
    this.hubConnection
      .invoke('UserTyping', toUser)
      .catch(err => console.error('Error sending typing notification:', err));
  }

  onTyping(callback: (fromUser: string) => void) {
    this.hubConnection.on('UserTyping', (fromUser: string) => {
      callback(fromUser);
    });

    //Methods for ChatBot
  }


}
