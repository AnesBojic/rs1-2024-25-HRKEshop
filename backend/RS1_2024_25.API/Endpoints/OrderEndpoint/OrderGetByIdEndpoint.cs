
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.SharedEnums;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.OrderEndpoint
{

    [Authorize]
    [Route("Orders/{orderId}")]
    public class OrderGetByIdEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<OrderGetByIdEndpoint.OrderGetByIdEndpointResponse>
    {
        [HttpGet]
        public override async Task<ActionResult<OrderGetByIdEndpointResponse>> HandleAsync([FromRoute]int orderId, CancellationToken cancellationToken = default)
        {
            

            var order = await db.Orders.Include(o=> o.User).Include(o=> o.Items)
                .ThenInclude(oi=>oi.ProductSize)
                    .ThenInclude(ps=> ps.Product)
                .Include(o=>o.Items)
                    .ThenInclude(oi=> oi.ProductSize)
                        .ThenInclude(ps=> ps.Size).FirstOrDefaultAsync(o => o.ID == orderId);
            var userId = db.GetUserIdThrow();

            if (order == null)
            {
               
                return NotFound("Order with this ID does not exist");
            }
            if (order.UserId != userId)
            {
                return BadRequest("Order exists but not for this user.");
            }

            var response = new OrderGetByIdEndpointResponse
            {
                OrderId = order.ID,
                OrderStatus = order.OrderStatus.ToString(),
                UserFullName = $"{order.User.Name} {order.User.Surname}",
                OrderDate = order.CreatedAt,
                TotalPrice = order.TotalAmount,
                OrderItems = order.Items.Select(oi => new OrderGetByIdEndpointResponse.OrderGetByIdEndpointResponseItem
                {
                    Name = oi.ProductSize.Product.Name,
                    Price = oi.UnitPrice,
                    Quantity = oi.Quantity


                }).ToList()



                
            };



            return Ok(response);



        }



        


        public class OrderGetByIdEndpointResponse
        {
            public required int OrderId { get; set; }
            public string OrderStatus { get; set; }

            public string UserFullName { get; set; }

            public DateTime OrderDate { get; set; }

            public List<OrderGetByIdEndpointResponseItem> OrderItems { get; set; } = new();

            public decimal TotalPrice { get; set; }


            public class OrderGetByIdEndpointResponseItem
            {
                public string Name { get; set; }
                
                public decimal Price { get; set; }

                public int Quantity { get; set; }

            }





        }
    }
}
