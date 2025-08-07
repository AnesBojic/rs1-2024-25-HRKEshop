using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SizeEndpoints
{

    [Authorize(Roles =("Manager,Admin"))]
    [Route("sizes/add")]
    public class SizeAddEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<SizeAddEndpoint.SizeAddRequest>
        .WithActionResult<SizeAddEndpoint.SizeAddResponse>
    {

        [HttpPost]
        public override async Task<ActionResult<SizeAddResponse>> HandleAsync([FromBody]SizeAddRequest request, CancellationToken cancellationToken = default)
        {

            var ifExistingSizeType = await db.SizeTypes.FirstOrDefaultAsync(st => st.ID == request.SizeTypeId);
            if (ifExistingSizeType == null)
            {

                return BadRequest("SizeType not found");
            }
            var ifSizeExistsAlready = await db.Sizes.Where(s => s.SizeTypeId == ifExistingSizeType.ID && s.Value.ToLower() == request.Value.ToLower()).AnyAsync();
            if (ifSizeExistsAlready)
            {
                return BadRequest("Size like this already exists.");
            }

            var newSize = new Size
            {
                SizeTypeId = request.SizeTypeId,
                Value = request.Value


            };

            db.SizesAll.Add(newSize);
            await db.SaveChangesAsync(cancellationToken);


            return Ok(new SizeAddResponse
            {
                ID = newSize.ID,
                Message = $"Size {newSize.Value} successfully created."

            });

            
            

        }







        public class SizeAddRequest
        {
            public required string Value { get; set; }

            public required int SizeTypeId { get; set; }


        }

        public class  SizeAddResponse
        {
            public int ID { get; set; }

            public string Message { get; set; }

        }

    }
}
