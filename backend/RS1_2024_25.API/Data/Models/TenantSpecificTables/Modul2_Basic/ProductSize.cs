using RS1_2024_25.API.Helper.BaseClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic
{
    public class ProductSize : TenantSpecificTable
    {
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        [Required]
        public Product Product { get; set; }

        public int SizeId { get; set; }
        [ForeignKey(nameof(SizeId))]
        [Required]
        public Size Size { get;set; }

        public decimal? Price { get; set; }

        public int Stock { get; set; }



    }
}
