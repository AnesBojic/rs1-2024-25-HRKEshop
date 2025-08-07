using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SizeEndpoints
{
    [Authorize(Roles ="Admin,Manager")]
    [Route("sizes")]
    public class SizeGetAllEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<SizeGetAllEndpoint.SizeGetAllRequest>
        .WithActionResult<MyPagedList<SizeGetAllEndpoint.SizeGetAllRequest>>
    {
        [HttpGet("all")]
        public override async Task<ActionResult<MyPagedList<SizeGetAllRequest>>> HandleAsync([FromBody]SizeGetAllRequest request, CancellationToken cancellationToken = default)
        {
            var query = db.Sizes.Include(s => s.SizeType).AsQueryable();
            
            if(!string.IsNullOrWhiteSpace(request.Q))
            {
                var normalized = request.Q.ToLower();

                query = query.Where(i =>
                i.Value.ToLower().Contains(normalized) ||
                i.SizeType.Name.ToLower().Contains(normalized));
            }

            var projected = query.Select(i =>
            new SizeGetAllResponse
            {
                Id = i.ID,
                Value = i.Value,
                SizeType = i.SizeType.Name




            });

            return Ok(await MyPagedList<SizeGetAllResponse>.CreateAsync(projected, request, cancellationToken));




        }








        public class SizeGetAllRequest : MyPagedRequest
        {
            public string? Q { get; set; }
        }

        public class SizeGetAllResponse
        {
            public int Id { get; set; }

            public string Value { get; set; }

            public string SizeType { get; set; }


        }

    }
}
