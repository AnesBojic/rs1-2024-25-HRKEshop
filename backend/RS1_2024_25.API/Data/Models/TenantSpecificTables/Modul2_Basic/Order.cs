using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Data.SharedEnums;
using RS1_2024_25.API.Helper.BaseClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic
{
    public class Order : TenantSpecificTable
    {

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }

        [Required]
        public string Address { get; set; }

        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? Phone { get; set; }

        [Required]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public bool IsPaid { get; set; } = false;

        public DateTime? PaymentDate { get; set; }


        public decimal TotalAmount { get; set; }

        public decimal? ShippingCost { get; set; }


        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();




    }
}
