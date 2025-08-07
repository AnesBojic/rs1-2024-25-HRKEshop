using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SizeTypeEndpoints
{

    [Authorize(Roles = "Manager,Admin")]
    [Route("SizeType/add")]
    public class SizeTypeAddEndpoint(ApplicationDbContext db) :MyEndpointBaseAsync
        .WithRequest<SizeTypeAddEndpoint.SizeTypeAddRequest>
        .WithActionResult<SizeTypeAddEndpoint.SizeTypeAddResponse>
    {
        [HttpPost]
        public override async Task<ActionResult<SizeTypeAddResponse>> HandleAsync([FromBody]SizeTypeAddRequest request, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Not valid name for the sizeType");
            }

            if(await db.SizeTypes.AnyAsync(st=> st.Name.ToLower() == request.Name.ToLower()))
            {

                return BadRequest("Size type with this name already exists");

            }

            var newSizeType = new SizeType
            {

                Name = request.Name


            };


            db.SizeTypesAll.Add(newSizeType);
            await db.SaveChangesAsync(cancellationToken);


            return Ok(new SizeTypeAddResponse
            {
                ID = newSizeType.ID,
                Message = $"Size type {newSizeType.Name} and ID {newSizeType.ID} created successfully"


            });










        }







        public class SizeTypeAddRequest
        {
            public required string Name { get; set; } = string.Empty;
        }

        public class SizeTypeAddResponse
        {
            public int ID { get; set; }

            public string Message { get; set; } = string.Empty;


        }
    }
}
