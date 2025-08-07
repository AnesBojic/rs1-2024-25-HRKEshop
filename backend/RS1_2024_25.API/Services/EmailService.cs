using MimeKit;
using MailKit.Net.Smtp;
using RS1_2024_25.API.Services.Interfaces;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;

namespace RS1_2024_25.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;
        private readonly AuthService _authservice;

        public EmailService(IConfiguration config,ApplicationDbContext db,AuthService authService)
        {
            _configuration = config;
            _db = db;
            _authservice = authService;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"]));

            email.To.Add(MailboxAddress.Parse(to));

            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:Port"]),
                true

                );

            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]
                );

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);


        }

        public async Task SendEmailVerificationAsync(AppUser appUser, CancellationToken cancellationToken = default)
        {
            var token = _authservice.GenerateEmailConfirmationToken();
            var encodedToken = Uri.EscapeDataString(token);

            var emailVerificationToken = new EmailVerificationToken
            {
                Token = token,
                UserId = appUser.ID,
                TenantId = appUser.TenantId,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(24),
                Used = false
            };
            _db.EmailVerificationTokensAll.Add(emailVerificationToken);

            var confirmationLink = $"{_configuration["FrontendBaseUrl"]}/auth/confirm-email?token={encodedToken}";

            await SendAsync(appUser.Email, "Verify your email",
                                    $"""
                    Click the button below to verify your email:<br><br>
                    <a href="{confirmationLink}" target="_blank">
                      <button style="padding:10px 15px; background-color:#4CAF50; color:white; border:none; border-radius:5px;">
                        Verify Email
                      </button>
                    </a>
                    """);

            await _db.SaveChangesAsync(cancellationToken);


        }
    }
}
