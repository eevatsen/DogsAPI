using dogs.Models;
using dogs.Services;
using dogs.Tests;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace dogs.Tests;

public class DogServiceTests
{
    [Fact]
    public async Task GetDogsAsync_ShouldReturnAllDogs()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);

        context.Dogs.Add(new Dog { Name = "Neo", Color = "red", TailLength = 22, Weight = 32 });
        context.Dogs.Add(new Dog { Name = "Jessy", Color = "black", TailLength = 7, Weight = 14 });
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetDogsAsync(new DogsQueryParams());

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetDogsAsync_WithSorting_ShouldReturnSortedDogs()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);

        context.Dogs.Add(new Dog { Name = "Neo", Color = "red", TailLength = 22, Weight = 32 });
        context.Dogs.Add(new Dog { Name = "Jessy", Color = "black", TailLength = 7, Weight = 14 });
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetDogsAsync(new DogsQueryParams
        {
            Attribute = "weight",
            Order = "asc"
        });

        // Assert
        Assert.Equal("Jessy", result[0].Name);
        Assert.Equal("Neo", result[1].Name);
    }

    [Fact]
    public async Task GetDogsAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);

        context.Dogs.Add(new Dog { Name = "Dog1", Color = "red", TailLength = 10, Weight = 20 });
        context.Dogs.Add(new Dog { Name = "Dog2", Color = "black", TailLength = 15, Weight = 25 });
        context.Dogs.Add(new Dog { Name = "Dog3", Color = "white", TailLength = 20, Weight = 30 });
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetDogsAsync(new DogsQueryParams
        {
            PageNumber = 2,
            PageSize = 1
        });

        // Assert
        Assert.Single(result);
        Assert.Equal("Dog2", result[0].Name);
    }

    [Fact]
    public async Task DogNameExistsAsync_WhenExists_ShouldReturnTrue()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);

        context.Dogs.Add(new Dog { Name = "Neo", Color = "red", TailLength = 22, Weight = 32 });
        await context.SaveChangesAsync();

        // Act
        var result = await service.DogNameExistsAsync("Neo");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DogNameExistsAsync_WhenNotExists_ShouldReturnFalse()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);

        // Act
        var result = await service.DogNameExistsAsync("NonExisting");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreateDogAsync_ShouldAddDogToDatabase()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);

        var request = new CreateDogRequest
        {
            Name = "Buddy",
            Color = "brown",
            TailLength = 15,
            Weight = 25
        };

        // Act
        var result = await service.CreateDogAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Buddy", result.Name);
        Assert.Equal(1, await context.Dogs.CountAsync());
    }
}