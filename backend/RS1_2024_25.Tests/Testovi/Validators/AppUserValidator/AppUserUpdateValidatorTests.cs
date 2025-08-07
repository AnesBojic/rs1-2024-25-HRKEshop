//using FluentValidation.TestHelper;
//using Microsoft.EntityFrameworkCore;
//using RS1_2024_25.API.Data;
//using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
//using RS1_2024_25.API.Endpoints.AppUserEndpoints;
//using RS1_2024_25.Tests.Services;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RS1_2024_25.Tests.Testovi.Validators.AppUserValidator
//{
//    public class AppUserUpdateValidatorTests
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly AppUserUpdateValidator _validator;

//        public AppUserUpdateValidatorTests()
//        {
//            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

//            _validator = new AppUserUpdateValidator(_db);

//        }

//        [Fact]
//        public async Task Should_have_no_Erros_When_everything_is_Empty_Only_Valid_Id()
//        {
//            var user = await _db.AppUsers.FirstOrDefaultAsync();

//            var newRequest = new AppUserUpdateEndpoint.AppUserUpdateRequest
//            {
//                ID = user.ID
//            };

//            var result = await _validator.TestValidateAsync(newRequest);

//            Assert.NotNull(result);
//            Assert.Equal(0, result.Errors.Count);
//        }
//        [Fact]
//        public async Task Should_have_NotExistintRoleError()
//        {

//            var user = await _db.AppUsers.FirstOrDefaultAsync();

//            var newRequest = new AppUserUpdateEndpoint.AppUserUpdateRequest
//            {
//                ID = user.ID,
//                RoleID = 5

//            };

//            var result = await _validator.TestValidateAsync(newRequest);

//            Assert.NotNull(result);
//            Assert.Equal(1, result.Errors.Count);
//            Assert.Contains("RoleID must refer to an existing role", result.Errors[0].ErrorMessage);

//        }

//        [Fact]
//        public async Task Should_have_error_when_Email_is_Invalid()
//        {

//            var user = await _db.AppUsers.FirstOrDefaultAsync();

//            var newRequest = new AppUserUpdateEndpoint.AppUserUpdateRequest
//            {
//                ID = user.ID,
//                Email = "nema"

//            };

//            var result = await _validator.TestValidateAsync(newRequest);

//            Assert.NotNull(result);
//            Assert.Equal(1, result.Errors.Count);
//            Assert.Contains("Email must be valid!", result.Errors.Select(e => e.ErrorMessage));



//        }
//        [Fact]
//        public async Task Should_have_error_when_Email_is_existing()
//        {
//            var noviUser = new AppUser
//            {
//                ID = 150,
//                Name="Imenjak",
//                Surname = "hudnjak",
//                Email = "imaemail@gmail.com"
//            };
//            noviUser.SetPassword("nesto123");


//            await _db.AppUsersAll.AddAsync(noviUser);
//            await _db.SaveChangesAsync();

//            Assert.NotNull(noviUser.TenantId);

//            var request = new AppUserUpdateEndpoint.AppUserUpdateRequest
//            {
//                ID = 1,
//                Email = "imaemail@gmail.com"

//            };

//            var result = await _validator.TestValidateAsync(request);

//            Assert.NotNull(result);
//            Assert.NotEmpty(result.Errors);
//            Assert.Equal("User with this email already exists", result.Errors[0].ErrorMessage);




//        }

//    }
//}
