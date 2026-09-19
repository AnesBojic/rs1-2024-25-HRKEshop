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
    public class ImageUploadEndpoint(ApplicationDbContext db, IFileService _iFileService, IAuthContext authContext) : MyEndpointBaseAsync
        .WithRequest<ImageUploadEndpoint.ImageUploadRequest>
        .WithActionResult<ImageUploadEndpoint.ImageUploadResponse>
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public override async Task<ActionResult<ImageUploadResponse>> HandleAsync([FromForm] ImageUploadRequest request, CancellationToken cancellationToken = default)
        {
            var imageableType = ImageHelper.Normalize(request.Imageabletype);

            if (!ImageHelper.isValid(imageableType))
            {
                throw new ArgumentException("Not valid type!");
            }

            if (!await ImageHelper.IsValidAssociation(db, imageableType, request.ImageableId, cancellationToken))
            {
                throw new ArgumentException("Invalid Id for the given ImageableType");
            }

            if (imageableType == ImageHelper.Users && request.ImageableId != authContext.AppUserId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You cannot upload image for another user");
            }

            if (imageableType == ImageHelper.Products && authContext.Role is not ("Admin" or "Manager"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You cannot upload product images");
            }

            var filePath = await _iFileService.SaveFileAsync(request.File, imageableType);
            var urlPath = _iFileService.GeneratePublicUrl(filePath);

            var image = new Image
            {
                Name = string.IsNullOrWhiteSpace(request.Name) ? request.File.FileName : request.Name.Trim(),
                ImageableId = request.ImageableId,
                ImageableType = imageableType,
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
            public string? Name { get; set; }

            public required int ImageableId { get; set; }

            public required string Imageabletype { get; set; }

            public required IFormFile File { get; set; }
        }

        public class ImageUploadResponse
        {
            public int ImageId { get; set; }
            public string Url { get; set; } = string.Empty;
        }
    }
}
