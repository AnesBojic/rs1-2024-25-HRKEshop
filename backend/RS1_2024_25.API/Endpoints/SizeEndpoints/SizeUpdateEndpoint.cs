using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.Payloads;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SizeEndpoints
{

    [Authorize(Roles = "Manager,Admin")]
    [Route("sizes/update")]
    public class SizeUpdateEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<SizeUpdateEndpoint.SizeUpdateRequest>
        .WithActionResult<SizeUpdateEndpoint.SizeUpdateResponse>
    {
        [HttpPut]
        public override async Task<ActionResult<SizeUpdateResponse>> HandleAsync([FromBody]SizeUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var isExisting = await db.Sizes.FirstOrDefaultAsync(s=> s.ID == request.Id);

            if (isExisting == null)
            {
                return NotFound("Size not found");
            }
            
            var isExistingSizeType = await db.SizeTypes.FirstOrDefaultAsync(st=> st.ID ==  request.SizeTypeId);
            if (isExistingSizeType == null)
            {
                return BadRequest("Size type does not exist");
            }
               
            
            var isDuplicate = await db.Sizes.AnyAsync(s=> s.ID != request.Id && s.SizeTypeId == request.SizeTypeId && s.Value.ToLower() == request.Value.ToLower());

            if(isDuplicate)
            {
                return BadRequest("Size with value asssociated with this type already exists");
            }

            isExisting.Value = request.Value;
            isExisting.SizeTypeId = request.SizeTypeId;

            await db.SaveChangesAsync(cancellationToken);

            return Ok(new SizeUpdateResponse
            {
                ID = isExisting.ID,
                Message = $"Size with the id {isExisting.ID} successfully updated"


            });









        }






        public class SizeUpdateRequest
        {
            public int Id { get; set; }

            public required string Value { get; set; }

            public required int SizeTypeId { get; set; }

        }
        public class SizeUpdateResponse : BaseResponse
        {
           
        }
    }
}
