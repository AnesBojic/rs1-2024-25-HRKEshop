using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.OrderEndpoint
{
    [Authorize(Roles = "Admin,Manager")]
    [Route("orders/admin/get-all")]
    public class OrderGetAllAdminEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<OrderGetAllAdminEndpoint.GetAllOrdersRequest>
        .WithActionResult<MyPagedList<OrderGetAllAdminEndpoint.GetAllOrdersResponse>>
    {
        [HttpGet]
        public override async Task<ActionResult<MyPagedList<GetAllOrdersResponse>>> HandleAsync([FromQuery]GetAllOrdersRequest request, CancellationToken cancellationToken = default)
        {

            var query = db.Orders.Include(o => o.User).Include(o => o.Items).AsQueryable();

            if(!string.IsNullOrEmpty(request.Q))
            {
                var q = request.Q.ToLower();


                query = query.Where(o =>
                    o.OrderStatus.ToString().ToLower().Contains(q) ||
                    o.User.Name.ToLower().Contains(q) ||
                    o.User.Surname.ToLower().Contains(q) ||
                    o.ID.ToString().ToLower().Contains(q) ||
                    o.CreatedAt.ToString("yyyy-MM-dd").Contains(q) ||
                    o.Address.ToLower().Contains(q)
                );

            }

            var pagedList = await MyPagedList<GetAllOrdersResponse.OrderDto>.CreateAsync(
                query.OrderByDescending(o => o.CreatedAt).Select(o => new GetAllOrdersResponse.OrderDto
                {
                    ID = o.ID,
                    FullName = o.User.Name + " " + o.User.Surname,
                    OrderDate = o.CreatedAt,
                    OrderStatus = o.OrderStatus.ToString(),
                    TotalAmount = o.TotalAmount,
                    TotalItemCount = o.Items.Sum(I => I.Quantity),
                    Email = o.User.Email



                }), request, cancellationToken);


            return Ok(pagedList);
        }




        public class GetAllOrdersRequest : MyPagedRequest
        {
            public string? Q { get; set; }
        }

        public class GetAllOrdersResponse
        {
           
            public class OrderDto
            {

                public int ID { get; set; }
                public string FullName { get; set; }

                public string Email { get; set; }

                public DateTime OrderDate { get; set; }

                public string OrderStatus { get; set; }

                public decimal TotalAmount { get; set; }

                public int TotalItemCount { get; set; }




            }




        }
    }
}
