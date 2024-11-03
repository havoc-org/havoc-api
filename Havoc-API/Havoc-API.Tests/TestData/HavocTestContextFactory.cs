using System;
using Havoc_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Havoc_API.Tests.TestData;

public static class HavocTestContextFactory
{
    public static HavocContext GetTestContext()
    {
        var options = new DbContextOptionsBuilder<HavocContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        var databaseContext = new HavocContext(options);
        databaseContext.Database.EnsureCreated();
        return databaseContext;
    }
}