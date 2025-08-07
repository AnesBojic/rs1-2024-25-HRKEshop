using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.SharedEnums;
using RS1_2024_25.API.Endpoints.Payloads;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.OrderEndpoint
{
    [Authorize(Roles = "Customer,Manager,Admin")]
    [Route("orders/cancel/{orderId}")]
    public class OrderCancelEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<OrderCancelEndpoint.OrderCancelResponse>
    {
        [HttpPut]
        public override async Task<ActionResult<OrderCancelResponse>> HandleAsync([FromRoute]int orderId, CancellationToken cancellationToken = default)
        {
            var userId = db.GetUserIdThrow();

            var order = await db.Orders.FirstOrDefaultAsync(o => o.ID == orderId && userId == o.UserId);

            if(order == null)
            {
                return NotFound("Order not found");
            }


            if(order.OrderStatus != OrderStatus.Pending)
            {
                return BadRequest("Only pending orders can be cancelled");
            }

            order.OrderStatus = OrderStatus.Cancelled;

            await db.SaveChangesAsync(cancellationToken);

            //Later extending authservice to check for admin and manager so they can also cancel order
            return Ok(new OrderCancelResponse
            {
                ID = order.ID,
                Message = "Order is successfully cancelled."

            });







            
        }

        public class OrderCancelResponse : BaseResponse
        {




        }
    }
}
