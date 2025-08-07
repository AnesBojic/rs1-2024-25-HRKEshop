using RS1_2024_25.API.Helper.BaseClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic
{
    public class OrderItem : TenantSpecificTable
    {
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        public int ProductSizeId { get; set; }

        [ForeignKey(nameof(ProductSizeId))]
        public ProductSize ProductSize { get; set; }


        [Range(1,int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0,double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public  decimal TotalPrice => Quantity * UnitPrice;

    }
}
