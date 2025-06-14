using Bogus;
using Microsoft.AspNetCore.Authorization;
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
    public class ImageUploadEndpoint(ApplicationDbContext db,IFileService _iFileService) : MyEndpointBaseAsync
        .WithRequest<ImageUploadRequest>
        .WithResult<string>
    {

        [HttpPost]
        public override async Task<string> HandleAsync([FromForm] ImageUploadRequest request, CancellationToken cancellationToken = default)
        {



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

            return $"Image uploaded successfully with an id {image.ID}";

            
        }



    }


    public class ImageUploadRequest
    {
        public required string Name { get; set; }

        public required int ImageableId { get; set; }

        public required string Imageabletype { get; set; }

        public required IFormFile File { get; set; }



    }
}
