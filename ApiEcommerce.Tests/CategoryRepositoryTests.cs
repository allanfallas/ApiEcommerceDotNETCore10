using System.Linq;
using ApiEcommerce.Models;
using ApiEcommerce.Repository;
using Xunit;

public class CategoryRepositoryTests
{
    [Fact]
    public void CreateCategory_Adds_Category()
    {
        using var db = TestUtilities.GetInMemoryDbContext();
        var repo = new CategoryRepository(db);

        var cat = new Category { Name = "Cat1" };
        var ok = repo.CreateCategory(cat);

        Assert.True(ok);
        Assert.Equal(1, db.Categories.Count());
    }

    [Fact]
    public void CategoryExists_ByName_Works()
    {
        using var db = TestUtilities.GetInMemoryDbContext();
        db.Categories.Add(new Category { Name = "MyCat" });
        db.SaveChanges();

        var repo = new CategoryRepository(db);
        Assert.True(repo.CategoryExists("MyCat"));
        Assert.False(repo.CategoryExists("Other"));
    }
}
