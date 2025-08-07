using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Helper.BaseClasses;
using System.ComponentModel.DataAnnotations.Schema;

namespace RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic
{
    public class Favorite : TenantSpecificTable
    {

        public int AppUserId { get; set; }
        [ForeignKey(nameof(AppUserId))]
        public AppUser AppUser { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }


    }
}
