using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Data.SharedEnums;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.OrderEndpoint
{

    [Authorize(Roles ="Customer")]
    [Route("orders/add")]
    public class OrderAddEndpoint(ApplicationDbContext db,IValidator<OrderAddEndpoint.OrderAddRequest> validator) : MyEndpointBaseAsync
        .WithRequest<OrderAddEndpoint.OrderAddRequest>
        .WithActionResult<OrderAddEndpoint.OrderAddResponse>
    {

        [HttpPost]
        public override async Task<ActionResult<OrderAddResponse>> HandleAsync([FromBody]OrderAddRequest request, CancellationToken cancellationToken = default)
        {
            var validationProblem = await FluentValidationHelper.TryValidateAsync(validator, request, cancellationToken);
            if (validationProblem != null)
            {
                return validationProblem;
            }
            var UserId = db.GetUserIdThrow();

            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);


            var productSizesIds = request.Items.Select(i => i.ProductSizeId).ToList();

            try
            {
                var productSizes = await db.ProductSizes.Include(ps => ps.Product).Where(ps => productSizesIds.Contains(ps.ID)).ToListAsync();

                

                var order = new Order
                {
                    UserId = UserId,
                    Address = request.ShippingAddress,
                    OrderStatus = OrderStatus.Pending,
                    IsPaid = false,
                    Items = new List<OrderItem>(),
                    


                };


                foreach (var item in request.Items)
                {
                    var productSize = productSizes.First(ps => ps.ID == item.ProductSizeId);

                    if (item.Quantity > productSize.Stock)
                    {
                        return BadRequest($"Not enough stock for ProductSize ID {productSize.ID}");
                    }


                    var unitPrice = productSize.Price;

                    order.Items.Add(new OrderItem
                    {
                        ProductSizeId = item.ProductSizeId,
                        Quantity = item.Quantity,
                        UnitPrice = (decimal)unitPrice!,
                       


                    });
                    
                   
                    productSize.Stock -= item.Quantity;

                }

                order.TotalAmount = order.Items.Sum(i=> i.TotalPrice);


                db.OrdersAll.Add(order);
                await db.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);


                return new OrderAddResponse
                {
                    ID = order.ID,
                    Message = $"Order created successfully! Order toal: {order.TotalAmount}$"


                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occured while processing your order.");
                
            }
            





        }






        public class OrderAddRequest
        {
            public string ShippingAddress { get; set; } = string.Empty;

            public List<Item> Items { get; set; } = new();


            public class Item
            {
                public int ProductSizeId { get; set; }

                public int Quantity { get; set; }

                
            }
        }

        public class OrderAddResponse
        {
            public int ID { get; set; }

            public string Message { get; set; } = string.Empty;

            
        }


    }
}
