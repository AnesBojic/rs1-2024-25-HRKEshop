using RS1_2024_25.API.Helper.BaseClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic
{
    public class Size : TenantSpecificTable
    {
        [Required]
        public string Value { get; set; }
        [Required]
        public int SizeTypeId {get;set;}

        [ForeignKey(nameof(SizeTypeId))]
        [Required]
        public SizeType SizeType { get; set; }
    }
}
