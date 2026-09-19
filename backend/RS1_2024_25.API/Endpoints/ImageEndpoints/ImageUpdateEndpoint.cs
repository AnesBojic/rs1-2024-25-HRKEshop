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
    public class ImageUpdateEndpoint(ApplicationDbContext db, IFileService _fileService, IAuthContext authContext) : MyEndpointBaseAsync
        .WithRequest<ImageUpdateRequest>
        .WithResult<ImageUpdateResponse>
    {
        [HttpPut]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public override async Task<ImageUpdateResponse> HandleAsync([FromForm] ImageUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var image = await db.ImagesAll.SingleOrDefaultAsync(i => i.ID == request.Id, cancellationToken);

            if (image == null)
            {
                throw new KeyNotFoundException("Image with this ID not found!");
            }

            var imageableType = ImageHelper.Normalize(request.ImageableType);

            if (!ImageHelper.isValid(imageableType))
            {
                throw new ArgumentException("Invalid imageable type");
            }
            if (!await ImageHelper.IsValidAssociation(db, imageableType, request.ImageableId, cancellationToken))
            {
                throw new ArgumentException("Invalid Id for the given ImageableType");
            }

            if (imageableType == ImageHelper.Users && request.ImageableId != authContext.AppUserId && authContext.Role != "Admin")
            {
                throw new UnauthorizedAccessException("You cannot update image for another user");
            }

            if (imageableType == ImageHelper.Products && authContext.Role is not ("Admin" or "Manager"))
            {
                throw new UnauthorizedAccessException("You cannot update product images");
            }

            image.Name = string.IsNullOrWhiteSpace(request.Name) ? image.Name : request.Name.Trim();
            image.ImageableId = request.ImageableId;
            image.ImageableType = imageableType;

            if (request.File != null && request.File.Length > 0)
            {
                _fileService.DeleteFile(image.FilePath);
                var newFilePath = await _fileService.SaveFileAsync(request.File, imageableType);
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
        public string ImageableType { get; set; } = string.Empty;
        public IFormFile? File { get; set; }
    }

    public class ImageUpdateResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
