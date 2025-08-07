using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services.Interfaces;

namespace RS1_2024_25.API.Endpoints.ImageEndpoints
{


    [Authorize]
    [Route("images/upload")]
    public class ImageUploadEndpoint(ApplicationDbContext db,IFileService _iFileService,IAuthContext authContext) : MyEndpointBaseAsync
        .WithRequest<ImageUploadEndpoint.ImageUploadRequest>
        .WithActionResult<ImageUploadEndpoint.ImageUploadResponse>
    {

        [HttpPost]
        public override async Task<ActionResult<ImageUploadResponse>> HandleAsync([FromForm] ImageUploadRequest request, CancellationToken cancellationToken = default)
        {

            if (request.Imageabletype == "users" && request.ImageableId != authContext.AppUserId)
                return Forbid("You cannot upload image for another user");



            if(!ImageHelper.isValid(request.Imageabletype))
            {
                throw new ArgumentException("Not valid type!");
            }
            //To check if the if the id exists for the user or product etc.. , can be expanded
            if(!await ImageHelper.IsValidAssociation(db,request.Imageabletype,request.ImageableId))
            {
                throw new ArgumentException("Invalid Id for the given ImageableType");

            }
            var filePath = await _iFileService.SaveFileAsync(request.File, request.Imageabletype);
            var urlPath = _iFileService.GeneratePublicUrl(filePath);



            var image = new Image
            {
                Name = request.Name.Trim(),
                ImageableId = request.ImageableId,
                ImageableType = request.Imageabletype.Trim(),
                FilePath = filePath,
                Url = urlPath

            };

            db.ImagesAll.Add(image);
            await db.SaveChangesAsync(cancellationToken);

            return Ok(new ImageUploadResponse
            {
                ImageId = image.ID,
                Url = image.Url
            });

            
        }
        public class ImageUploadRequest
        {
            public required string Name { get; set; }

            public required int ImageableId { get; set; }

            public required string Imageabletype { get; set; }

            public required IFormFile File { get; set; }

        }
        public class ImageUploadResponse
        {

            public int ImageId { get; set; }
            public string Url { get; set; }

        }


    }


    

}
