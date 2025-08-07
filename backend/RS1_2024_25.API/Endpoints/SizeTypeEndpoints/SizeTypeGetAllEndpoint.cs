using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SizeTypeEndpoints
{

    [Authorize(Roles ="Admin,Manager")]
    [Route("sizetypes")]
    public class SizeTypeGetAllEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithoutRequest
        .WithActionResult<List<SizeTypeGetAllEndpoint.SizeTypeGetAllResponse>>
    {
        [HttpGet("all")]
        public override async Task<ActionResult<List<SizeTypeGetAllResponse>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var listOfSizeTypes = await db.SizeTypes.ToListAsync(cancellationToken);

            

            var listDto = listOfSizeTypes.Select(st => new SizeTypeGetAllResponse
            {
                ID = st.ID,
                Name = st.Name
            }).ToList();


            return Ok(listDto);

        }





        public class SizeTypeGetAllResponse
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

    }
}
