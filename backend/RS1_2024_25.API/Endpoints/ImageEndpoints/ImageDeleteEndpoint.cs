using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using System.IO;

namespace RS1_2024_25.API.Endpoints.ImageEndpoints
{
    [Authorize]
    [Route("images")]
    public class ImageDeleteEndpoint(ApplicationDbContext db,FileService fileService) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithoutResult
    {
        [HttpDelete("{id}")]
        public override async Task HandleAsync([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var image = await db.ImagesAll.FirstOrDefaultAsync(i => i.ID == id, cancellationToken);

            if (image == null)
            {
                throw new KeyNotFoundException("Image not found");
            }

            if (!string.IsNullOrEmpty(image.FilePath))
            {

                if (System.IO.File.Exists(image.FilePath))
                {
                    fileService.DeleteFile(image.FilePath);

                }

                db.Remove(image);

                await db.SaveChangesAsync(cancellationToken);


            }


        }
    }
}