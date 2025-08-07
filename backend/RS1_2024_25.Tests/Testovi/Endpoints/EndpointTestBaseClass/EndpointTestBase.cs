using Microsoft.AspNetCore.Http;
using RS1_2024_25.API.Data;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass
{
    public abstract class EndpointTestBase : IDisposable
    {

        protected readonly ApplicationDbContext _db;
        protected readonly HttpContext _httpContext;

        protected EndpointTestBase(string role = "Customer")
        {
            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser(role: role);
            _db = TestApplication1DbContext.CreateAsync(accessor).GetAwaiter().GetResult();


            _httpContext = accessor.HttpContext;

        }





        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
