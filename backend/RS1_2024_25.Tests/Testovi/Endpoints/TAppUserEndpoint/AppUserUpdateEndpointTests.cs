//using Microsoft.AspNetCore.Mvc;
//using RS1_2024_25.API.Data;
//using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
//using RS1_2024_25.API.Endpoints.AppUserEndpoints;
//using RS1_2024_25.Tests.Services;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RS1_2024_25.Tests.Testovi.Endpoints.AppUserEndpoint
//{
//    public class AppUserUpdateEndpointTests
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly AppUserUpdateEndpoint _endpoint;
//        private readonly AppUserUpdateValidator _validator;



//        public AppUserUpdateEndpointTests()
//        {

//            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

//            _validator = new AppUserUpdateValidator(_db);

//            _endpoint = new AppUserUpdateEndpoint(_db,_validator);

//            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();


//            _endpoint.ControllerContext = new ControllerContext
//            {

//                HttpContext = accessor.HttpContext

//            };

//        }


//        [Fact]
//        public async Task HandleAsync_ShouldupdateUser_WhenRequestIsValid()
//        {
//            //Dodajemo novog usera za update

//            var user = new AppUser
//            {
//                Name = "ProbaUsera",
//                Surname = "ProbaUsera",
//                Email = "probaUsera@gmail.com",
//                Phone = "06235521",
//                RoleID = 3
//            };
//            user.SetPassword("test");

//            _db.AppUsersAll.Add(user);
//            await _db.SaveChangesAsync();

//            var idUseraUpdated = user.ID;


//            //Changing some values

//            var request = new AppUserUpdateEndpoint.AppUserUpdateRequest
//            {
//                ID = idUseraUpdated,
//                Name = "Promjena",
//                Surname = "Promjena",
//            };


//            var result = await _endpoint.HandleAsync(request);
//            Assert.NotNull(result);
//            Assert.IsType<NoContentResult>(result);

//            var updatedUser =  _db.AppUsers.FirstOrDefault(u=> u.ID == idUseraUpdated);

//            Assert.NotNull(updatedUser);
//            Assert.Equal(idUseraUpdated, updatedUser.ID);
//            Assert.Equal("Promjena", updatedUser.Name);
//            Assert.Equal("Promjena", updatedUser.Surname);
//            Assert.Equal("probaUsera@gmail.com", updatedUser.Email);

//            //Checking when nothing is provided but Id


//            var request1 = new AppUserUpdateEndpoint.AppUserUpdateRequest
//            {
//                ID = idUseraUpdated


//            };

//            var result1 = await _endpoint.HandleAsync(request1);

//            Assert.NotNull(result1);
//            Assert.IsType<NoContentResult>(result1);


//            var updatedUser1 = _db.AppUsers.FirstOrDefault(u => u.ID == idUseraUpdated);

//            Assert.NotNull(updatedUser1);
//            Assert.Equal("Promjena", updatedUser1.Name);
//            Assert.Equal("Promjena", updatedUser1.Surname);
//            Assert.Equal("probaUsera@gmail.com", updatedUser1.Email);





//        }
//        [Fact]
//        public async Task HandleAsync_UserDoesNotExist_ShouldReturnNotFound()
//        {
//            var request = new AppUserUpdateEndpoint.AppUserUpdateRequest
//            {

//                ID = -22
//            };

//            var res = await _endpoint.HandleAsync(request);

//            Assert.IsType<NotFoundObjectResult>(res);


//        }




//    }
//}
