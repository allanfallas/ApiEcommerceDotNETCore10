using System.Linq;
using ApiEcommerce.Models;
using ApiEcommerce.Repository;
using Xunit;

public class ProductRepositoryTests
{
    [Fact]
    public void CreateProduct_Adds_Product_And_Returns_True()
    {
        using var db = TestUtilities.GetInMemoryDbContext();
        var repo = new ProductRepository(db);

        var category = new Category { Name = "Cat" };
        var product = new Product { Name = "Test", Price = 10, Stock = 5, Category = category, CategoryId = category.Id, SKU = "SKU1" };
        var result = repo.CreateProduct(product);

        Assert.True(result);
        Assert.Equal(1, db.Products.Count());
    }

    [Fact]
    public void BuyProduct_Decrements_Stock_When_Available()
    {
        using var db = TestUtilities.GetInMemoryDbContext();
        var category = new Category { Name = "C" };
        db.Categories.Add(category);
        db.Products.Add(new Product { Name = "P1", Price = 1, Stock = 10, Category = category, CategoryId = category.Id, SKU = "SKU_P1" });
        db.SaveChanges();

        var repo = new ProductRepository(db);
        var result = repo.BuyProduct("P1", 3);

        Assert.True(result);
        Assert.Equal(7, db.Products.First().Stock);
    }

    [Fact]
    public void GetProductsInPages_Returns_Correct_Page()
    {
        using var db = TestUtilities.GetInMemoryDbContext();
        for (int i = 1; i <= 10; i++) db.Products.Add(new Product { Name = "P" + i, Price = i, Stock = i, Category = new Category { Name = "c" + i }, CategoryId = i, SKU = "SKU" + i });
        db.SaveChanges();

        var repo = new ProductRepository(db);
        var page = repo.GetProductsInPages(2, 3);

        Assert.Equal(3, page.Count);
        Assert.Equal("P4", page.First().Name);
    }

    [Fact]
    public void SearchProducts_Finds_By_Name_And_Description()
    {
        using var db = TestUtilities.GetInMemoryDbContext();
        db.Products.Add(new Product { Name = "Apple", Description = "Fresh red", Category = new Category { Name = "fr" }, CategoryId = 1, SKU = "SKU_A" });
        db.Products.Add(new Product { Name = "Banana", Description = "Yellow fruit", Category = new Category { Name = "fr2" }, CategoryId = 2, SKU = "SKU_B" });
        db.SaveChanges();

        var repo = new ProductRepository(db);
        var results = repo.SearchProducts("red");

        Assert.Single(results);
        Assert.Equal("Apple", results.First().Name);
    }
}
