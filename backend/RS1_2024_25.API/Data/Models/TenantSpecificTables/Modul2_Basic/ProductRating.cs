using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Helper.BaseClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic
{
    public class ProductRating : TenantSpecificTable
    {
        public int AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [Range(1,5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }



    }
}
