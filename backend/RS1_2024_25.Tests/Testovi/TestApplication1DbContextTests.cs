using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi
{
    public class TestApplication1DbContextTests
    {
        [Fact]
        public async Task CreateAsync_ShouldSeedData_ReturnsSeededUsers()
        {

            var dbcontext = await TestApplication1DbContext.CreateAsync();

            var usersCount = dbcontext.AppUsersAll.Count();
            var tenantsCount = dbcontext.Tenants.Count();
            var rolesCount = dbcontext.Roles.Count();
            var imagesCount = dbcontext.ImagesAll.Count();

            Assert.True(tenantsCount > 0, "Tenants were not seeded");
            Assert.True(rolesCount > 0, "Roles were not seeded");
            Assert.True(usersCount > 0, "Users were not seeded");
            Assert.True(imagesCount > 0, "Images were not seeeded");


        }

    }
}
