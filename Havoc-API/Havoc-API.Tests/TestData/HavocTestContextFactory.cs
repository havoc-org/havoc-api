using System;
using Havoc_API.Data;
using Havoc_API.DTOs.Assignment;
using Havoc_API.DTOs.Attachment;
using Havoc_API.DTOs.Comment;
using Havoc_API.DTOs.Tag;
using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.TaskStatus;
using Havoc_API.DTOs.User;
using Havoc_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Task = Havoc_API.Models.Task;
using TaskStatus = Havoc_API.Models.TaskStatus;

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