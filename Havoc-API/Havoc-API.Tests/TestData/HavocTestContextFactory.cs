using System;
using Havoc_API.Data;
using Microsoft.EntityFrameworkCore;

namespace Havoc_API.Tests.TestData;

public static class HavocTestContextFactory
{
    public static HavocContext GetTestContext()
    {
        var options = new DbContextOptionsBuilder<HavocContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var databaseContext = new HavocContext(options);
        databaseContext.Database.EnsureCreated();
        return databaseContext;
    }
}