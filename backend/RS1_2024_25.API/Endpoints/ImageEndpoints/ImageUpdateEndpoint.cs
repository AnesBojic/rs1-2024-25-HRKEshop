using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services.Interfaces;

namespace RS1_2024_25.API.Endpoints.ImageEndpoints
{

    [Authorize]
    [Route("images/update")]
    public class ImageUpdateEndpoint(ApplicationDbContext db,IFileService _fileService) : MyEndpointBaseAsync
        .WithRequest<ImageUpdateRequest>
        .WithResult<ImageUpdateResponse>
    {
        [HttpPut]
        [Consumes("multipart/form-data")]
        public override async Task<ImageUpdateResponse> HandleAsync([FromForm] ImageUpdateRequest request,CancellationToken cancellationToken = default)
        {

            var image = await db.ImagesAll.SingleOrDefaultAsync(i => i.ID == request.Id);

            if(image == null)
            {
                throw new KeyNotFoundException("Image with this ID not found!");
            }
            if(!ImageHelper.isValid(request.ImageableType))
            {
                throw new ArgumentException("Invalid imageable type");
            }
            if(!await ImageHelper.IsValidAssociation(db,request.ImageableType,request.ImageableId))
            {
                throw new ArgumentException("Invalid Id for the given ImageableType");
            }
            
            


            image.Name = request.Name ?? image.Name.Trim();
            image.ImageableId = request.ImageableId;
            image.ImageableType = request.ImageableType;

            if(request.File != null && request.File.Length > 0)
            {

                _fileService.DeleteFile(image.FilePath);

                var newFilePath = await _fileService.SaveFileAsync(request.File, request.ImageableType);
                image.FilePath = newFilePath;
                image.Url = _fileService.GeneratePublicUrl(newFilePath);

            }

            await db.SaveChangesAsync(cancellationToken);

            return new ImageUpdateResponse
            {
                Id = image.ID,
                Message = $"Successfully updated image {image.ID}"


            };







        }





    }

    public class ImageUpdateRequest
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        public int ImageableId { get; set; }

        public string ImageableType { get; set; }

        public IFormFile? File { get; set; }



    }

    

    public class ImageUpdateResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;

    }

}
