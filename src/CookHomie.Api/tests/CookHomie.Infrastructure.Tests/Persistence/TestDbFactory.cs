using CookHomie.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CookHomie.Infrastructure.Tests.Persistence;

public static class TestDbFactory
{
    public static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}