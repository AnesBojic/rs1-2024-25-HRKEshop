using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Services
{
    public class JwtTestHttpContextAccessorHelper
    {
        public static IHttpContextAccessor CreateWithJwtAuthenticatedUser(string userId="55",string role ="Admin",string tenantId="1",string email="asim_asim@gmail.com")
        {
            var mocKHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var context = new DefaultHttpContext();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userId),
                new Claim(ClaimTypes.Name,"Test user"),
                new Claim(ClaimTypes.Email,email),
                new Claim("tenant_id",tenantId),
                new Claim(ClaimTypes.Role,role)
            };

            var identity = new ClaimsIdentity(claims,"testJwt");
            var principal = new ClaimsPrincipal(identity);

            context.User = principal;

            mocKHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);


            return mocKHttpContextAccessor.Object;

            




        }



    }
}
