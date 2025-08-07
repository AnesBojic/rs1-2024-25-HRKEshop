using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SizeTypeEndpoints
{

    [Authorize(Roles = "Manager,Admin")]
    [Route("sizetypes/update")]
    public class SizeTypeUpdateEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<SizeTypeUpdateEndpoint.SizeTypeUpdateRequest>
        .WithActionResult<SizeTypeUpdateEndpoint.SizeTypeUpdateResponse>
    {
        [HttpPut]
        public override async Task<ActionResult<SizeTypeUpdateResponse>> HandleAsync([FromBody]SizeTypeUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var sizeTypeToUpdate = await db.SizeTypes.FirstOrDefaultAsync(st => st.ID == request.Id);


            if (sizeTypeToUpdate == null)
            {
                return NotFound("No sizetype with specified Id");
            }
            if(string.IsNullOrWhiteSpace(request.UpdatedName))
            {

                return BadRequest("Not valid name for sizeType");
            }

            var sizeTypeExistingName = await db.SizeTypes.AnyAsync(st => st.Name.ToLower() == request.UpdatedName.ToLower() && st.ID != request.Id);

            if(sizeTypeExistingName)
            {

                return BadRequest("Size type with this name already exists.");
            }


            sizeTypeToUpdate.Name = request.UpdatedName;

            await db.SaveChangesAsync();

            return Ok(new SizeTypeUpdateResponse
            {
                Id = sizeTypeToUpdate.ID,
                Message = $"Size type with id {sizeTypeToUpdate.ID} successfuly updated to : {sizeTypeToUpdate.Name}"


            });








        }









        public class SizeTypeUpdateRequest
        {
            public required int Id { get; set; }

            public string UpdatedName { get; set; }


        }
        public class SizeTypeUpdateResponse
        {
            public int Id { get; set; }

            public string Message { get; set; } = string.Empty;


        }


    }
}
