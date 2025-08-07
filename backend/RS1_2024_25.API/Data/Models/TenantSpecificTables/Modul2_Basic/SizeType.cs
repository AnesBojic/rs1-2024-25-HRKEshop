using RS1_2024_25.API.Helper.BaseClasses;
using System.ComponentModel.DataAnnotations;

namespace RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic
{
    public class SizeType : TenantSpecificTable
    {
        [Required]
        public string Name { get; set; }

        public ICollection<Size> Sizes { get; set; } = new List<Size>();

    }
}
