using FluentValidation;
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
    

    public class AppUserAddEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly AppUserAddEndpoint _endpoint;
        private readonly AppUserAddValidator validator;

        public AppUserAddEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            validator = new AppUserAddValidator(_db);

            _endpoint = new AppUserAddEndpoint(_db,validator);

            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();

            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext


            };
        }

        [Fact]
        public async Task HandleAsync_ShouldAddNewUser_WhenRequestIsValid()
        {

            var request = new AppUserAddEndpoint.AppUserAddRequest
            {
                Name ="Novi1",
                Surname = "NoviPrezime1",
                Email = "noviEmail@gmail.com",
                Password = "test123",
                Phone = "12345678",
            };

            var response = await _endpoint.HandleAsync(request);

            Assert.NotNull(response);
            Assert.True(response.Value.ID > 0);
            Assert.Equal("User created-- :D", response.Value.Message);




            var savedUser = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == request.Email);
            Assert.NotNull(savedUser);
            Assert.Equal(request.Name, savedUser.Name);
            Assert.Equal(request.Surname, savedUser.Surname);
            Assert.Equal(request.Phone, savedUser.Phone);

            //Test provjera baze cisto da vidim nesto

            var users = await _db.AppUsers.ToListAsync();
            var brojKorisnikaSaTrenutnimTenantId = users.Count();

            foreach(var user in users)
            {

                Console.WriteLine($"{user.Name} {user.Email} TenantId: {user.TenantId}");

            }

        }
        [Fact]
        public async Task HandleAsync_ShouldThrowBadRequest_WhenValidationFails()
        {
            var request = new AppUserAddEndpoint.AppUserAddRequest
            {
                Name = "ime",
                Surname = "prezime",
                Email = "najnonvijiEmail",
                Password = "nema",
                Phone = "06123523"
            };

            var response = await _endpoint.HandleAsync(request);

            Assert.NotNull(response);
            Assert.IsType<BadRequestObjectResult>(response.Result);

            var badRequest = response.Result as BadRequestObjectResult;

            Assert.NotNull(badRequest);

            var errors = badRequest.Value as Dictionary<string, string[]>;

            Assert.NotNull(errors);
            Assert.True(errors.ContainsKey("Password"));
        }

    }

    
}
