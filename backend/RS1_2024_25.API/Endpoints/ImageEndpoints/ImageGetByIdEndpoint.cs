
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.ImageEndpoints
{

    [Authorize]
    [Route("images/{id:int}")]
    public class ImageGetByIdEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithResult<ImageGetByIdResponse>
    {
        [HttpGet]
        public override async Task<ImageGetByIdResponse> HandleAsync([FromRoute]int id, CancellationToken cancellationToken = default)
        {
            var image = await db.ImagesAll.FirstOrDefaultAsync(i => i.ID == id,cancellationToken);

            if(image == null)
            {
                throw new KeyNotFoundException("Image not found");
            }

            return new ImageGetByIdResponse
            {
                Name = image.Name,
         
                Url = image.Url,


            };



        }

    }


    public class ImageGetByIdResponse
    {
        public required string Name { get; set; }

        public string? Url { get; set; }


    }
}
