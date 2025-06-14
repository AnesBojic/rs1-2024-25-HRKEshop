using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;
using static RS1_2024_25.API.Endpoints.AppUserEndpoints.AppUserGetAllEndpoint;

namespace RS1_2024_25.API.Endpoints.AppUserEndpoints
{

    [Authorize(Roles = "Admin,Manager")]
    [Route("appusers")]
    public class AppUserGetAllEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<AppUserGetAllRequest>
        .WithResult<MyPagedList<AppUserGetAllResponse>>
    {

        [HttpGet("filter")]
        public override async Task<MyPagedList<AppUserGetAllResponse>> HandleAsync([FromQuery] AppUserGetAllRequest request, CancellationToken cancellationToken = default)
        {

            var query = db.AppUsers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Q))
            {
                query = query.Where(x =>

                    x.Name.ToLower().Contains(request.Q.ToLower()) ||
                    x.Surname.ToLower().Contains(request.Q.ToLower()) ||
                    x.Email.ToLower().Contains(request.Q.ToLower()) ||
                    x.Phone.ToLower().Contains(request.Q.ToLower())

                );
            }


            var projected = query.Select(x => new AppUserGetAllResponse
            {
                ID = x.ID,
                Name = x.Name,
                Surname = x.Surname,
                Email = x.Email,
                Phone = x.Phone,
                RoleName = x.Role.Name,
                IsLocked = x.isLocked()
            });

            return await MyPagedList<AppUserGetAllResponse>.CreateAsync(projected, request, cancellationToken);
        }
        public class AppUserGetAllRequest : MyPagedRequest
        {
            public string? Q { get; set; }
        }
        public class AppUserGetAllResponse
        {
            public required int ID { get; set; }
            public required string Name { get; set; }
            public required string Surname { get; set; }
            public required string Email { get; set; }
            public string? Phone { get; set; }
            public required string RoleName { get; set; }
            public bool IsLocked { get; set; }


        }




    }
}
