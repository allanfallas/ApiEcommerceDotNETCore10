using System.Collections.Generic;
using System.Threading.Tasks;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

public class UserRepositoryTests
{
    [Fact]
    public void IsUniqueUser_Works()
    {
        using var db = TestUtilities.GetInMemoryDbContext();
        db.ApplicationUsers.Add(new ApplicationUser { UserName = "exist" });
        db.SaveChanges();

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { { "ApiSettings:SecretKey", "secret" } }).Build();
        var userManager = MockUserManager();
        var roleManager = MockRoleManager();
        var mapper = new Mock<IMapper>().Object;

        var repo = new UserRepository(db, config, userManager.Object, roleManager.Object, mapper);

        Assert.False(repo.IsUniqueUser("exist"));
        Assert.True(repo.IsUniqueUser("newuser"));
    }

    [Fact]
    public async Task Register_Creates_User_And_Returns_UserData()
    {
        using var db = TestUtilities.GetInMemoryDbContext();
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { { "ApiSettings:SecretKey", "secret" } }).Build();

        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);
        userManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .Callback<ApplicationUser, string>((u, _) => { db.ApplicationUsers.Add(u); db.SaveChanges(); })
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        var roleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
        roleManager.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        roleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<UserDataDto>(It.IsAny<ApplicationUser>())).Returns((ApplicationUser u) => new UserDataDto { UserName = u.UserName, Name = u.Name });

        var repo = new UserRepository(db, configuration, userManager.Object, roleManager.Object, mapperMock.Object);
        var createDto = new CreateUserDto { UserName = "testuser", Password = "Pass123$", Name = "Test" };

        var result = await repo.Register(createDto);

        Assert.NotNull(result);
        Assert.Equal("testuser", result.UserName);
    }

    private static Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
        return mgr;
    }

    private static Mock<RoleManager<IdentityRole>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
    }
}
