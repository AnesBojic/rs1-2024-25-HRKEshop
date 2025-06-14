using FluentAssertions;
using FluentValidation.TestHelper;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Endpoints.AppUserEndpoints;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Validators.AppUserValidator
{
    public class AppUserAddValidatorTests
    {
        private readonly ApplicationDbContext _db;
        private readonly  AppUserAddValidator _validator;


        public AppUserAddValidatorTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            
            _validator = new AppUserAddValidator(_db);

        }


        [Fact]

        public async Task Should_have_Error_When_Email_Is_Invalid()
        {

            var request = new AppUserAddEndpoint.AppUserAddRequest
            {
                Name = "Test",
                Surname = "User",
                Email = "invalid",
                Password = "Pass123!",
                Phone = "062352523"
            };

            var result = await _validator.TestValidateAsync(request);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }
        [Fact]
        public async Task Should_Pass_For_Valid_Request()
        {
            var request = new AppUserAddEndpoint.AppUserAddRequest
            {
                Name = "Name",
                Surname = "Surname",
                Email = "Samir@gmail.com",
                Password = "pass123",
                Phone = "062352315"



            };
            var result = await _validator.TestValidateAsync(request);
            result.ShouldNotHaveAnyValidationErrors();

        }
        [Fact]
        public async Task Should_Have_Error_When_Email_Already_Exists()
        {
            var userForTest = new AppUser
            {
                Name = "Test",
                Surname = "Test",
                Email = "test@gmail.com",
                Phone = "0612351254",



            };
            userForTest.SetPassword("test12345");

            _db.AppUsersAll.Add(userForTest);

            await _db.SaveChangesAsync();


            var request = new AppUserAddEndpoint.AppUserAddRequest
            {
                Name = "nesto",
                Surname = "nesto2",
                Email = "test@gmail.com",
                Password = "23125215251",
                Phone = "061235123"

            };

            var result = await _validator.TestValidateAsync(request);
            result.ShouldHaveValidationErrorFor(e => e.Email);




        }


    }
}
