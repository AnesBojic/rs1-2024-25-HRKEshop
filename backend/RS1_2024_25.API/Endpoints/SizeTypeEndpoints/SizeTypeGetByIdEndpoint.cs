using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SizeTypeEndpoints
{

    [Authorize(Roles = "Admin,Manager")]
    [Route("sizetype/{sizetypeid}")]
    public class SizeTypeGetByIdEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<SizeTypeGetByIdEndpoint.SizeTypeGetByIdEndpointResponse>
    {
        [HttpGet]
        public override async Task<ActionResult<SizeTypeGetByIdEndpointResponse>> HandleAsync([FromRoute]int sizetypeid, CancellationToken cancellationToken = default)
        {
            var sizeType = await db.SizeTypes.FirstOrDefaultAsync(st => st.ID == sizetypeid);

            if(sizeType == null)
            {
                return NotFound("Size type not found");  
            }


            return Ok(new SizeTypeGetByIdEndpointResponse
            {
                ID = sizeType.ID,
                Name = sizeType.Name
            });




        }







        public class SizeTypeGetByIdEndpointResponse
        {
            public int ID { get; set; }
            public string Name { get; set; } = string.Empty;


        }

    }
}
