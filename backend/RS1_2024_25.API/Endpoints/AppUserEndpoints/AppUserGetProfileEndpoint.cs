using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services.Interfaces;

namespace RS1_2024_25.API.Endpoints.AppUserEndpoints
{
    [Authorize]
    [Route("appusers/profile")]
    public class AppUserGetProfileEndpoint(ApplicationDbContext db, IAuthContext auth) : MyEndpointBaseAsync
        .WithoutRequest
        .WithActionResult<AppUserGetProfileEndpoint.AppUserGetProfileResponse>
    {
        [HttpGet]
        public override async Task<ActionResult<AppUserGetProfileResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var userId = auth.AppUserId;

            var user = await db.AppUsers.FirstOrDefaultAsync(au=> au.ID ==  userId);

            if(user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new AppUserGetProfileResponse
            {
                ID = user.ID,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                City = user.City ?? "",
                Address = user.Address ?? "",
                ZipCode = user.ZipCode ?? "",
                Phone = user.Phone ?? ""

            });




        }


        public class AppUserGetProfileResponse
        {
            public int ID { get; set; }

            public string Name { get; set; }

            public string Surname { get; set; }

            public string Email { get; set; }

            public string Address { get; set; }
            public string City { get; set; }

            public string ZipCode { get; set; }

            public string Phone { get; set; }

            
        }




    }
}
