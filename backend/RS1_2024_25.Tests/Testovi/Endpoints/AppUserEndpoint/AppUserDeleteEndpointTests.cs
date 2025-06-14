using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.AppUserEndpoints;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.AppUserEndpoint
{

     


    public class AppUserDeleteEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly AppUserDeleteEndpoint _endpoint;

        public AppUserDeleteEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            _endpoint = new AppUserDeleteEndpoint(_db);


            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();


            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext
            };
        }



        [Fact] 

        public async Task HandleAsync_UserExists_DeleteSuccessfully()
        {
            var users = await _db.AppUsers.ToListAsync();

            var startingNumberOfUsers = users.Count();

            Assert.NotNull(users);

            var userToDelete = await _db.AppUsers.FirstOrDefaultAsync();

            Assert.NotNull(userToDelete);
            var idOfDeletedUser = userToDelete.ID;


            var res = await _endpoint.HandleAsync(idOfDeletedUser);

            var usersOnEnd = await _db.AppUsers.ToListAsync();

            var endingNumberOfUsers = usersOnEnd.Count();
            
            var deletedUser = await _db.AppUsers.FirstOrDefaultAsync(u=> u.ID ==  idOfDeletedUser);

            Assert.NotEqual(startingNumberOfUsers, endingNumberOfUsers);
            Assert.Null(deletedUser);

            var result = Assert.IsType<NoContentResult>(res);
            Assert.NotNull(result);
           


        }

        [Fact]
        public async Task HandleAsync_ReturnsNotFound_WhenUserDoesNotExist()
        {

            var result = await _endpoint.HandleAsync(-555);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            Assert.NotNull(notFound);
            Assert.Equal("User not found", notFound.Value);





        }


    }
}
