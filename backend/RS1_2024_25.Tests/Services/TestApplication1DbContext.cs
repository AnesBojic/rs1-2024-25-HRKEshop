using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.DataSeedEndpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Services
{
    public class TestApplication1DbContext
    {
        public static async Task<ApplicationDbContext> CreateAsync(CancellationToken cancellationToken=default)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            IHttpContextAccessor httpContextAccessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();

            var dbContext = new ApplicationDbContext(options, httpContextAccessor);

            var seeder = new DataSeedBaseSeed(dbContext);
            await seeder.HandleAsync(cancellationToken);


            return dbContext;

        }


    }
}
