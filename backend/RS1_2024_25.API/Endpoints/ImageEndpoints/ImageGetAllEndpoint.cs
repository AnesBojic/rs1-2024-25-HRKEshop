using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.ImageEndpoints
{

    [Authorize(Roles = "Admin")]
    [Route("images")]
    public class ImageGetAllEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithoutRequest
        .WithResult<List<ImageGetAllResponse>>
    {
        [HttpGet("all")]
        public override async Task<List<ImageGetAllResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await db.ImagesAll.Select(img =>
            new ImageGetAllResponse
            {
                Name = img.Name,
                FilePath = img.FilePath,
                Url = img.Url


            }).ToListAsync(cancellationToken);



        }



    }


    public class ImageGetAllResponse
    {
        public string Name { get; set; }

        public string? FilePath { get; set; }

        public string? Url { get; set; }


    }
}
