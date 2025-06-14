using Microsoft.AspNetCore.Identity;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Helper.BaseClasses;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth
{
    public class AppUser : TenantSpecificTable
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public DateTime? EmailVerifiedAt { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        public string? RememberToken { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? ZipCode { get; set; }

        public string? Phone { get; set; }


        [Column("roles_id")]
        public int RoleID { get; set; }

        [ForeignKey(nameof(RoleID))]
        public Role  Role { get; set; }


        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LockoutUntil { get; set; }

        public void SetPassword(string password)
        {
            var hasher = new PasswordHasher<AppUser>();
            PasswordHash = hasher.HashPassword(this, password);
        }

        public bool VerifiyPassword(string password)
        {
            var hasher = new PasswordHasher<AppUser>();
            var result = hasher.VerifyHashedPassword(this, PasswordHash, password);

            if(result == PasswordVerificationResult.Success)
            {
                FailedLoginAttempts = 0;
                LockoutUntil = null;
                return true;
            }
            else
            {
                FailedLoginAttempts++;
                return false;
            }
        }

        public bool isLocked()
        {
            return LockoutUntil.HasValue && LockoutUntil.Value > DateTime.UtcNow;
        }

        public void LockAccount(int minutes)
        {
            LockoutUntil = DateTime.UtcNow.AddMinutes(minutes);
        }



    }
}
