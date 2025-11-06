using dogs.Data;
using Microsoft.EntityFrameworkCore;

namespace dogs.Tests;

public static class TestHelpers
{
    public static DogsDbContext GetMockDbContext()
    {
        var options = new DbContextOptionsBuilder<DogsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DogsDbContext(options);
    }
}