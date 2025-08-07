using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Endpoints.AppUserEndpoints;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.AppUserEndpoint
{
    public class AppUserGetAllEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly AppUserGetAllEndpoint _endpoint;


        public AppUserGetAllEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            _endpoint = new AppUserGetAllEndpoint(_db);



            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();



            _endpoint.ControllerContext = new ControllerContext
            {

                HttpContext = accessor.HttpContext

            };

        }
        [Fact]
        public async Task HandleAsync_NoFilter_ReturnsPaginatedUsers()
        {

            var request = new AppUserGetAllEndpoint.AppUserGetAllRequest
            {
                PageNumber = 1,
                PageSize =5
            };

            var result = await _endpoint.HandleAsync(request);

            var numberOfUsersForThisTenant = await _db.AppUsers.ToListAsync();
            




            Assert.NotNull(result);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(numberOfUsersForThisTenant.Count, result.TotalCount);
            Assert.Equal(request.PageSize, result.DataItems.Length);
        }

        [Fact]

        public async Task HandleAsync_WithFilter_ReturnsOneUserExactly()
        {
            var user = new AppUser
            {
                Name = "TestOneUser",
                Surname = "TestOneUser",
                RoleID = 3,
                Email = "testOneUser@gmail.com",
                Phone = "066666666"
            };
            user.SetPassword("test");

            _db.AppUsersAll.Add(user);
            await _db.SaveChangesAsync();

            var request = new AppUserGetAllEndpoint.AppUserGetAllRequest
            {
                Q = "TestOneUser",
                PageNumber = 1,
                PageSize = 5
            };

            var result = await _endpoint.HandleAsync(request);


            Assert.NotEmpty(result.DataItems);
            Assert.Single(result.DataItems);
            Assert.Equal("TestOneUser", result.DataItems[0].Name);
        }

        [Fact]

        public async Task HandleAsync_RequestPageBeyondRange_ReturnsEmpty()
        {

            var request = new AppUserGetAllEndpoint.AppUserGetAllRequest
            {
                PageNumber = 1500,
                PageSize = 10,



            };
            var result = await _endpoint.HandleAsync(request);

            Assert.Empty(result.DataItems);
        }


        [Fact]

        public async Task HandleAsync_Filter_IsCaseSensitive()
        {
            var user = new AppUser
            {
                Name = "Maxi33",
                Surname = "Maxi33",
                RoleID = 3,
                Phone = null,
                Email = "Maxi33@gmail.com"
            };
            user.SetPassword("test");
            _db.AppUsersAll.Add(user);
            await _db.SaveChangesAsync();

            var request = new AppUserGetAllEndpoint.AppUserGetAllRequest
            {
                Q = "maxi33",
                PageNumber = 1,
                PageSize = 5,


            };

            var result = await _endpoint.HandleAsync(request);

            Assert.Single(result.DataItems);
            Assert.Equal("Maxi33", result.DataItems[0].Name);





        }



    }
}
