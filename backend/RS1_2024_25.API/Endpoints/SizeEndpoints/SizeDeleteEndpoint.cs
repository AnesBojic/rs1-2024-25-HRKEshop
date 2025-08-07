using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.Payloads;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SizeEndpoints
{

    [Authorize(Roles = "Manager,Admin")]
    [Route("sizes/{id}")]
    public class SizeDeleteEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<SizeDeleteEndpoint.SizeDeleteEndpointResponse>
    {
        [HttpDelete]
        public override async Task<ActionResult<SizeDeleteEndpointResponse>> HandleAsync([FromRoute]int id, CancellationToken cancellationToken = default)
        {
            var existingItem = await db.Sizes.FirstOrDefaultAsync(s => s.ID == id);
            if (existingItem == null)
                return NotFound("No size with that id");

            var foreignConstraintSize = await db.ProductSizes.AnyAsync(ps => ps.SizeId == id);

            if(foreignConstraintSize)
            {
                return BadRequest("Size is foreign key in one or more objects");
            }
            //maybe later soft delete implemented

            db.Remove(existingItem);
            await db.SaveChangesAsync(cancellationToken);


            var idToDelete = existingItem.ID;

            return Ok(new SizeDeleteEndpointResponse
            {
                ID = idToDelete,
                Message = "Size successfully deleted."

            });



        }








        public class SizeDeleteEndpointResponse : BaseResponse
        {


        }
    }
}
