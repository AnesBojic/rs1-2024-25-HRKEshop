using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;

namespace RS1_2024_25.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);

        Task SendEmailVerificationAsync(AppUser appUser, CancellationToken cancellationToken = default);
    }
    
}
