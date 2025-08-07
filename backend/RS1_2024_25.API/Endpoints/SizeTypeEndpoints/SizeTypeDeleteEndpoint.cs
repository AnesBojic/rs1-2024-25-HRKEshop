using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SizeTypeEndpoints
{
    [Authorize(Roles = "Admin,Manager")]
    [Route("sizetype/delete/{id:int}")]
    public class SizeTypeDeleteEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<string>
    {
        [HttpDelete]
        public override async Task<ActionResult<string>> HandleAsync([FromRoute]int id, CancellationToken cancellationToken = default)
        {
            var sizeTypeForDeletion = await db.SizeTypes.FirstOrDefaultAsync(st => st.ID == id);

            if(sizeTypeForDeletion == null)
            {
                return NotFound("No size type found");
            }

            var hasSizes = await db.Sizes.AnyAsync(s => s.SizeTypeId == id);

            if(hasSizes)
            {
                return BadRequest("Cannot delete sizeType because it is used by one or more sizes");
            };

            db.Remove(sizeTypeForDeletion);
            await db.SaveChangesAsync(cancellationToken);

            return Ok("SizeType deleted succesfully");





        }



    }
}
