using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.OrderEndpoint
{
    [Authorize(Roles ="Customer")]
    [Route("orders")]
    public class OrderGetAllEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithoutRequest
        .WithActionResult<List<OrderGetAllEndpoint.OrdersGetAllResponse>>

    {
        [HttpGet]
        public override async Task<ActionResult<List<OrdersGetAllResponse>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var userId = db.GetUserIdThrow();

            var orders = await db.Orders.Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrdersGetAllResponse
                {
                    OrderId = o.ID,
                    OrderDate = o.CreatedAt,
                    TotalPrice = o.TotalAmount,
                    OrderStatus = o.OrderStatus.ToString(),
                    ItemCount = o.Items.Sum(i=> i.Quantity)




                }).ToListAsync();


            return Ok(orders);



        }








        public class OrdersGetAllResponse
        {
            public int OrderId { get; set; }

            public DateTime OrderDate { get; set; }

            public decimal TotalPrice { get; set; }

            public string OrderStatus { get; set; }

            public int ItemCount { get; set; }


        }
    }
}
