using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SizeEndpoints
{
    [Authorize]
    [Route("sizes/{sizeId}")]
    public class SizeGetByIdEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<SizeGetByIdEndpoint.SizeGetByIdResponse>
    {

        [HttpGet]
        public override async Task<ActionResult<SizeGetByIdResponse>> HandleAsync([FromRoute]int sizeId, CancellationToken cancellationToken = default)
        {

            var size = await db.Sizes.Include(S=> S.SizeType).FirstOrDefaultAsync(s => s.ID == sizeId);

            if(size == null)
            {
                return NotFound("Size does not exist.");
            }

            return Ok(new SizeGetByIdResponse
            {
                Id = size.ID,
                Value = size.Value,
                SizeType = size.SizeType.Name

            });


        }
        







        public class SizeGetByIdResponse
        {
            public int Id { get; set; }
            public string Value { get; set; } = string.Empty;

            public string SizeType { get; set; } = string.Empty;


        }
    }
}
