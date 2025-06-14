using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.ImageEndpoints
{

    [Authorize]
    [Route("images/by-entity")]
    public class ImageGetByEntityEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<ImageGetByEntityRequest>
        .WithResult<List<ImageGetByEntityResponse>>
    {
        [HttpGet]
        public override async Task<List<ImageGetByEntityResponse>> HandleAsync([FromQuery]ImageGetByEntityRequest request, CancellationToken cancellationToken = default)
        {
            string normalizedType = request.ImageableType.Trim().ToLower();

            if(!ImageHelper.isValid(normalizedType))
            {
                throw new ArgumentException("Invalid imageable type");
            }

            //Checking entity for the provided ImageableId, needs to be expanded later, also need to add validation for invalid imageableId
            bool entityExists = normalizedType switch
            {
                "users" => await db.AppUsersAll.AnyAsync(u => u.ID == request.ImageableId, cancellationToken),
                "roles" => await db.Roles.AnyAsync(r => r.ID == request.ImageableId, cancellationToken),
                _=> false
            };
           
            if (!entityExists)
            {
                throw new ArgumentException("Entity with given Id does not exist");
            }

            var images = await db.ImagesAll.Where(img => img.ImageableId == request.ImageableId && img.ImageableType.ToLower() == normalizedType).OrderBy(img=>img.CreatedAt)
                .Select(img => new ImageGetByEntityResponse
                {
                    Id = img.ID,
                    Name = img.Name,
                    Url = img.Url

                }).ToListAsync(cancellationToken);


            return images;


            
        }



    }


    public class ImageGetByEntityRequest
    {
        [FromQuery]
        public required  int ImageableId { get; set; }
        [FromQuery]
        public required string ImageableType { get; set; }


    }

    public class ImageGetByEntityResponse
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string? Url { get; set; }

    }
}
