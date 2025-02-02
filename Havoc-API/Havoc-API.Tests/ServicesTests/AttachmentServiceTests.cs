using Havoc_API.Data;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Havoc_API.DTOs.Attachment;
using FluentAssertions;
using Xunit;
using Havoc_API.Models;

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
    public async System.Threading.Tasks.Task AddAttachmentAsync_ShouldAddAttachment_WhenAttachmentDoesntExist()
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
    public async System.Threading.Tasks.Task AddAttachmentAsync_ShouldNotsAddAttachment_WhenAttachmentExist()
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
    public async System.Threading.Tasks.Task AddManyAttachmentAsync_ShouldAddAllAttachments_WhenAttachmentsDontExist()
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
    public async System.Threading.Tasks.Task AddManyAttachmentAsync_ShouldAddOnlyNewAttachments_WhenSomeOfAttachmentsExist()
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

    [Fact]
    public async System.Threading.Tasks.Task DeleteAttachmentAsync_ShouldDeleteAttachment_WhenAttachmentExists()
    {
        //Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);

        var attachmentToDelete = AttachmentFactory.Create(user, task, "TestFileLink1");
        var attachments = new[] {
            attachmentToDelete,
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
        int attachmentId = attachmentToDelete.AttachmentId;
        //Act
        var result = await _attachmentService.DeleteAttachmentAsync(
            attachmentId,
            task.TaskId,
            project.ProjectId
        );

        //Assert
        result.Should().BePositive();
        _context.Attachments.Count().Should().Be(attachmentCount - 1);
        _context.Attachments.FirstOrDefault(a => a.AttachmentId == attachmentId).Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAttachmentAsync_ShouldReturnAttachment_WhenAttachmentExists()
    {
        //Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var attachment = AttachmentFactory.Create(user, task);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Attachments.AddAsync(attachment);
        await _context.SaveChangesAsync();

        //Act
        var result = await _attachmentService.GetAttachmentAsync(attachment.AttachmentId, task.TaskId, project.ProjectId);

        //Assert
        result.Should().BeOfType<AttachmentGET>();
        result.FileLink.Should().Be(attachment.FileLink);
        result.AttachmentId.Should().Be(attachment.AttachmentId);
        result.User.UserId.Should().Be(user.UserId);
        result.User.FirstName.Should().Be(user.FirstName);
        result.User.LastName.Should().Be(user.LastName);
        result.User.Email.Should().Be(user.Email);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTasksAttachmentsAsync_ShouldReturnAllTasksAttachment_WhenAtLeastOneAttachmentExists()
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

        //Act
        var result = await _attachmentService.GetTasksAttachmentsAsync(task.TaskId);

        //Assert
        result.Should().BeAssignableTo<IEnumerable<AttachmentGET>>();

        var resultSet = result.Zip(attachments, Tuple.Create);
        foreach (var dataAndGet in resultSet)
        {
            dataAndGet.Item1.FileLink.Should().Be(dataAndGet.Item2.FileLink);
            dataAndGet.Item1.AttachmentId.Should().Be(dataAndGet.Item2.AttachmentId);
            dataAndGet.Item1.User.UserId.Should().Be(dataAndGet.Item1.User.UserId);
            dataAndGet.Item1.User.FirstName.Should().Be(user.FirstName);
            dataAndGet.Item1.User.LastName.Should().Be(user.LastName);
            dataAndGet.Item1.User.Email.Should().Be(user.Email);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTasksAttachmentsAsync_ShouldReturnEmaptyIEnumerable_WhenAttachmentsDontExist()
    {
        //Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        _context.Attachments.RemoveRange(_context.Attachments.ToList());
        await _context.SaveChangesAsync();

        //Act
        var result = await _attachmentService.GetTasksAttachmentsAsync(task.TaskId);

        //Assert
        result.Should().BeAssignableTo<IEnumerable<AttachmentGET>>();
        result.Should().BeEmpty();
    }
}