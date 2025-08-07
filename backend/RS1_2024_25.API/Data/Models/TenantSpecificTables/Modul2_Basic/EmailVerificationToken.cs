using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Helper.BaseClasses;

namespace RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic
{
    public class EmailVerificationToken: TenantSpecificTable
    {
       
        public string Token { get; set; } = default!;

        public DateTime ExpiresAtUtc { get; set; }

        public bool Used { get; set; }

        public int UserId { get; set; }

        public AppUser User { get; set; } = default!;

    }
}
