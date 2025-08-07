using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Endpoints.DataSeedEndpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Services
{
    public class TestApplication1DbContext
    {
        public static async Task<ApplicationDbContext> CreateAsync(IHttpContextAccessor? httpContextAccessor = null,CancellationToken cancellationToken=default)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            httpContextAccessor ??= JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();

            var dbContext = new ApplicationDbContext(options, httpContextAccessor);

            var seeder = new DataSeedBaseSeed(dbContext);
            await seeder.HandleAsync(cancellationToken);

            var userId = int.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            


            return dbContext;

        }


    }
}
