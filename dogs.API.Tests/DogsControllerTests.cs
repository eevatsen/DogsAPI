using dogs.Tests;
using dogs.Controllers;
using dogs.Models;
using dogs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace dogs.Tests;

public class DogsControllerTests
{
    [Fact]
    public void Ping_ShouldReturnCorrectVersion()
    {
        // Arrange
        var mockContext = TestHelpers.GetMockDbContext();
        var service = new DogService(mockContext);
        var controller = new DogsController(service);

        // Act
        var result = controller.Ping() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dogshouseservice.Version1.0.1", result.Value);
    }

    [Fact]
    public async Task GetDogs_ShouldReturnListOfDogs()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        // Add test dogs
        context.Dogs.Add(new Dog { Name = "Neo", Color = "red&amber", TailLength = 22, Weight = 32 });
        context.Dogs.Add(new Dog { Name = "Jessy", Color = "black&white", TailLength = 7, Weight = 14 });
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetDogs(new DogsQueryParams()) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var returnedDogs = result.Value as List<Dog>;
        Assert.NotNull(returnedDogs);
        Assert.Equal(2, returnedDogs.Count);
    }

    [Fact]
    public async Task GetDogs_WithSorting_ShouldReturnSortedDogs()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        context.Dogs.Add(new Dog { Name = "Neo", Color = "red", TailLength = 22, Weight = 32 });
        context.Dogs.Add(new Dog { Name = "Jessy", Color = "black", TailLength = 7, Weight = 14 });
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetDogs(new DogsQueryParams
        {
            Attribute = "weight",
            Order = "asc"
        }) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var dogs = result.Value as List<Dog>;
        Assert.NotNull(dogs);
        Assert.Equal("Jessy", dogs[0].Name); // lighter dog first
        Assert.Equal("Neo", dogs[1].Name);
    }

    [Fact]
    public async Task GetDogs_WithInvalidAttribute_ShouldReturnBadRequest()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        // Act
        var result = await controller.GetDogs(new DogsQueryParams { Attribute = "invalid_field" });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetDogs_WithInvalidOrder_ShouldReturnBadRequest()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        // Act
        var result = await controller.GetDogs(new DogsQueryParams { Order = "invalid" });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetDogs_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        context.Dogs.Add(new Dog { Name = "Dog1", Color = "red", TailLength = 10, Weight = 20 });
        context.Dogs.Add(new Dog { Name = "Dog2", Color = "blue", TailLength = 15, Weight = 25 });
        context.Dogs.Add(new Dog { Name = "Dog3", Color = "green", TailLength = 20, Weight = 30 });
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetDogs(new DogsQueryParams
        {
            PageNumber = 2,
            PageSize = 1
        }) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var dogs = result.Value as List<Dog>;
        Assert.NotNull(dogs);
        Assert.Single(dogs);
        Assert.Equal("Dog2", dogs[0].Name);
    }

    [Fact]
    public async Task CreateDog_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        var request = new CreateDogRequest
        {
            Name = "Buddy",
            Color = "brown",
            TailLength = 15,
            Weight = 25
        };

        // Act
        var result = await controller.CreateDog(request) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var dog = result.Value as Dog;
        Assert.NotNull(dog);
        Assert.Equal("Buddy", dog.Name);
        Assert.Equal("brown", dog.Color);
        Assert.Equal(15, dog.TailLength);
        Assert.Equal(25, dog.Weight);
    }

    [Fact]
    public async Task CreateDog_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        // Add existing dog
        context.Dogs.Add(new Dog { Name = "Neo", Color = "red", TailLength = 10, Weight = 20 });
        await context.SaveChangesAsync();

        var request = new CreateDogRequest
        {
            Name = "Neo",
            Color = "brown",
            TailLength = 15,
            Weight = 25
        };

        // Act
        var result = await controller.CreateDog(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateDog_WithNegativeTailLength_ShouldReturnBadRequest()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        var request = new CreateDogRequest
        {
            Name = "BadDog",
            Color = "brown",
            TailLength = -5,
            Weight = 25
        };

        // Simulate model validation
        controller.ModelState.AddModelError("TailLength", "Tail length must be zero or positive");

        // Act
        var result = await controller.CreateDog(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateDog_WithZeroWeight_ShouldReturnBadRequest()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        var request = new CreateDogRequest
        {
            Name = "ThinDog",
            Color = "brown",
            TailLength = 10,
            Weight = 0
        };

        // Simulate model validation
        controller.ModelState.AddModelError("Weight", "Weight must be positive");

        // Act
        var result = await controller.CreateDog(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateDog_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        var request = new CreateDogRequest
        {
            Name = "",
            Color = "brown",
            TailLength = 10,
            Weight = 20
        };

        // Simulate model validation
        controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await controller.CreateDog(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetDogs_WithSortingAndPagination_ShouldWork()
    {
        // Arrange
        var context = TestHelpers.GetMockDbContext();
        var service = new DogService(context);
        var controller = new DogsController(service);

        context.Dogs.Add(new Dog { Name = "Dog1", Color = "red", TailLength = 30, Weight = 30 });
        context.Dogs.Add(new Dog { Name = "Dog2", Color = "blue", TailLength = 10, Weight = 10 });
        context.Dogs.Add(new Dog { Name = "Dog3", Color = "green", TailLength = 20, Weight = 20 });
        await context.SaveChangesAsync();

        // Act - sort by tail_length descending, get page 1 with size 2
        var result = await controller.GetDogs(new DogsQueryParams
        {
            Attribute = "tail_length",
            Order = "desc",
            PageNumber = 1,
            PageSize = 2
        }) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var dogs = result.Value as List<Dog>;
        Assert.NotNull(dogs);
        Assert.Equal(2, dogs.Count);
        Assert.Equal("Dog1", dogs[0].Name); // longest tail first
        Assert.Equal("Dog3", dogs[1].Name); // second longest
    }
}