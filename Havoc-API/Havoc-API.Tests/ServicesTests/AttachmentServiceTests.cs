using Havoc_API.Data;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Havoc_API.DTOs.Attachment;
using FluentAssertions;
using Xunit;

namespace Havoc_API.Tests.ServicesTests;

public class AttachmentServiceTests
{
    private readonly IHavocContext _context;
    private readonly IAttachmentService _attachmentService;
    public AttachmentServiceTests()
    {
        _context = HavocTestContextFactory.GetTestContext();

        //SUT
        _attachmentService = new AttachmentService(_context);
    }

    [Fact]
    public async Task AddAttachmentAsync_ShouldAddAttachment_WhenAttachmentDoesntExist()
    {
        //Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var attachment = AttachmentFactory.Create(user, task);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        int attachmentCount = _context.Attachments.Count();
        //Act
        var result = await _attachmentService.AddAttachmentAsync(
            AttachmentFactory.CreatePost(user.UserId, attachment.FileLink),
            task.TaskId,
            user.UserId,
            project.ProjectId
        );

        //Assert
        result.Should().BeOfType<AttachmentGET>();
        result.FileLink.Should().Be(attachment.FileLink);
        result.User.UserId.Should().Be(user.UserId);
        _context.Attachments.Count().Should().Be(attachmentCount + 1);
    }

    [Fact]
    public async Task AddAttachmentAsync_ShouldNotsAddAttachment_WhenAttachmentExist()
    {
        //Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var attachment = AttachmentFactory.Create(user, task);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        await _attachmentService.AddAttachmentAsync(
            AttachmentFactory.CreatePost(user.UserId, attachment.FileLink),
            task.TaskId,
            user.UserId,
            project.ProjectId
        );

        int attachmentCount = _context.Attachments.Count();
        //Act
        var result = await _attachmentService.AddAttachmentAsync(
            AttachmentFactory.CreatePost(user.UserId, attachment.FileLink),
            task.TaskId,
            user.UserId,
            project.ProjectId
        );

        //Assert
        result.Should().BeOfType<AttachmentGET>();
        result.FileLink.Should().Be(attachment.FileLink);
        result.User.UserId.Should().Be(user.UserId);
        _context.Attachments.Count().Should().Be(attachmentCount);
    }

    [Fact]
    public async Task AddManyAttachmentAsync_ShouldAddAllAttachments_WhenAttachmentsDontExist()
    {
        //Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var attachments = new[] {
            AttachmentFactory.Create(user, task, "TestFileLink1"),
            AttachmentFactory.Create(user, task, "TestFileLink2"),
            AttachmentFactory.Create(user, task, "TestFileLink3"),
            AttachmentFactory.Create(user, task, "TestFileLink4"),
            AttachmentFactory.Create(user, task, "TestFileLink5")
        };

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Attachments.AddRangeAsync(attachments);
        await _context.SaveChangesAsync();

        int attachmentCount = _context.Attachments.Count();
        //Act
        var newAttachments = new List<AttachmentPOST> {
            AttachmentFactory.CreatePost(user.UserId,"TestFileLink6"),
            AttachmentFactory.CreatePost(user.UserId, "TestFileLink7"),
            AttachmentFactory.CreatePost(user.UserId, "TestFileLink8"),
            AttachmentFactory.CreatePost(user.UserId, "TestFileLink9"),
            AttachmentFactory.CreatePost(user.UserId, "TestFileLink10")
        };

        var result = await _attachmentService.AddManyAttachmentsAsync(
            newAttachments,
            task.TaskId,
            user.UserId,
            project.ProjectId
        );

        //Assert
        Console.WriteLine(result.GetType());
        result.Should().BeAssignableTo<IEnumerable<AttachmentGET>>();
        var resultSet = result.Zip(newAttachments, Tuple.Create);
        foreach (var GetAndPost in resultSet)
        {
            GetAndPost.Item1.FileLink.Should().Be(GetAndPost.Item2.FileLink);
            GetAndPost.Item1.User.UserId.Should().Be(GetAndPost.Item1.User.UserId);
        }
        _context.Attachments.Count().Should().Be(attachmentCount + newAttachments.Count);
    }

    [Fact]
    public async Task AddManyAttachmentAsync_ShouldAddOnlyNewAttachments_WhenSomeOfAttachmentsExist()
    {
        //Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);

        var attachments = new[] {
            AttachmentFactory.Create(user, task, "TestFileLink1"),
            AttachmentFactory.Create(user, task, "TestFileLink2"),
            AttachmentFactory.Create(user, task, "TestFileLink3"),
            AttachmentFactory.Create(user, task, "TestFileLink4"),
            AttachmentFactory.Create(user, task, "TestFileLink5")
        };

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Attachments.AddRangeAsync(attachments);
        await _context.SaveChangesAsync();

        int attachmentCount = _context.Attachments.Count();

        var newAttachments = new List<AttachmentPOST> {
            AttachmentFactory.CreatePost(user.UserId,"TestFileLink6"),
            AttachmentFactory.CreatePost(user.UserId, "TestFileLink8"),
            AttachmentFactory.CreatePost(user.UserId, "TestFileLink10")
        };

        int numberOfNewAttachments = newAttachments.Count;
        newAttachments.AddRange(attachments.Select(a => AttachmentFactory.CreatePost(user.UserId, a.FileLink)));

        //Act
        var result = await _attachmentService.AddManyAttachmentsAsync(
            newAttachments,
            task.TaskId,
            user.UserId,
            project.ProjectId
        );

        //Assert
        result.Should().BeAssignableTo<IEnumerable<AttachmentGET>>();
        newAttachments.Should().HaveCount(numberOfNewAttachments + attachments.Length);

        var resultSet = result.Zip(newAttachments, Tuple.Create);
        foreach (var GetAndPost in resultSet)
        {
            GetAndPost.Item1.FileLink.Should().Be(GetAndPost.Item2.FileLink);
            GetAndPost.Item1.User.UserId.Should().Be(GetAndPost.Item1.User.UserId);
        }
        _context.Attachments.Should().HaveCount(attachmentCount + numberOfNewAttachments);
    }
}