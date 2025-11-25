import { Component } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

@Component({
    selector: 'app-chatbot',
    templateUrl: './chatbot.component.html',
    styleUrls: ['./chatbot.component.css'],
    standalone: false
})
export class ChatbotComponent {
  isOpen = false;
  userMessage = '';
  messages: { sender: 'user' | 'bot', text: string }[] = [];
  isBotTyping = false;

  private readonly apiUrl = 'http://localhost:7000/messagess/send';

  constructor(private http: HttpClient) {}

  toggleChat() {
    this.isOpen = !this.isOpen;
  }

  sendMessage() {
    const message = this.userMessage.trim();
    if (!message) return;

    // Dodaj korisničku poruku odmah
    this.messages.push({ sender: 'user', text: message });
    this.userMessage = '';

    // Aktiviraj typing indikator
    this.isBotTyping = true;

    this.http.post<{ response: string }>(this.apiUrl, { message })
      .subscribe({
        next: (res) => {
          this.isBotTyping = false;
          this.messages.push({ sender: 'bot', text: res.response });
        },
        error: (err: HttpErrorResponse) => {
          this.isBotTyping = false;
          console.error('Chatbot error:', err);

          let errorMessage = '⚠️ Došlo je do greške.';

          if (err.status === 429) {
            errorMessage = '⚠️ Chatbot je preopterećen. Molimo sačekajte nekoliko sekundi prije novog pokušaja.';
          }
          else if (err.status === 503) {
            errorMessage = '⚠️ Chatbot trenutno nije dostupan. Molimo pokušajte kasnije.';
          }
          else if (err.status === 0) {
            errorMessage = '⚠️ Ne mogu se povezati sa serverom.';
          }

          this.messages.push({
            sender: 'bot',
            text: errorMessage
          });
        }
      });
  }
}
