using FluentAssertions;
using Havoc_API.Data;
using Havoc_API.DTOs.Comment;
using Havoc_API.Models;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Xunit;

namespace Havoc_API.Tests.ServicesTests;
public class CommentServiceTests
{
    private readonly IHavocContext _context;
    private readonly ICommentService _commentService;
    public CommentServiceTests()
    {
        _context = HavocTestContextFactory.GetTestContext();
        _commentService = new CommentService(_context);
    }
    [Fact]
    public async void AddCommentAsync_ShouldAddComment_AndReturnCommentGet()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = Havoc_API.Tests.TestData.TaskFactory.Create(user, project);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var commentCount = _context.Comments.Count();
        var comment = CommentFactory.CreatePost();

        // Act
        var result = await _commentService.AddCommentAsync(comment, user.UserId, task.TaskId);

        // Assert
        _context.Comments.Count().Should().Be(commentCount + 1);
        result.Should().BeOfType<CommentGET>();
        result.Content.Should().Be(comment.Content);
        result.CommentDate.Should().BeCloseTo(DateTime.Now, new TimeSpan(0, 0, 1));
        result.User.UserId.Should().Be(user.UserId);
        result.User.FirstName.Should().Be(user.FirstName);
        result.User.LastName.Should().Be(user.LastName);
        result.User.Email.Should().Be(user.Email);
    }

    [Fact]
    public async void DeleteCommentAsync_ShouldDeleteComment_AndReturnNumberOfChangedRows()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = Havoc_API.Tests.TestData.TaskFactory.Create(user, project);
        var comment = CommentFactory.Create(user, task);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        var commentCount = _context.Comments.Count();

        // Act
        var result = await _commentService.DeleteCommentAsync(comment.CommentId, project.ProjectId);
        // Assert
        _context.Comments.Count().Should().Be(commentCount - 1);
        _context.Comments.FirstOrDefault(c => c.CommentId == comment.CommentId).Should().BeNull();
        result.Should().BePositive();
    }

    [Fact]
    public async void GetTasksCommentsAsync_ShouldReturnIEnumerableOfTasksComments_WhenCommentsOfTaskExist()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = Havoc_API.Tests.TestData.TaskFactory.Create(user, project);
        var comments = new List<Comment>()
        {
            CommentFactory.Create(user, task),
            CommentFactory.Create(user, task),
            CommentFactory.Create(user, task, "Test Comment 2"),
            CommentFactory.Create(user, task, "Test Comment 3"),
            CommentFactory.Create(user, task, "Test Comment 4")
        };

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();

        // Act
        var result = await _commentService.GetTasksCommentsAsync(task.TaskId);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeAssignableTo<IEnumerable<CommentGET>>();

        var resultSet = result.Zip(comments, Tuple.Create);
        foreach (var dataAndGet in resultSet)
        {
            dataAndGet.Item1.Content.Should().Be(dataAndGet.Item2.Content);
            dataAndGet.Item1.CommentId.Should().Be(dataAndGet.Item2.CommentId);
            dataAndGet.Item1.CommentDate.Should().BeCloseTo(dataAndGet.Item2.CommentDate, new TimeSpan(0, 0, 1));
            dataAndGet.Item1.User.UserId.Should().Be(dataAndGet.Item1.User.UserId);
            dataAndGet.Item1.User.FirstName.Should().Be(user.FirstName);
            dataAndGet.Item1.User.LastName.Should().Be(user.LastName);
            dataAndGet.Item1.User.Email.Should().Be(user.Email);
        }
    }

    [Fact]
    public async void GetTasksCommentsAsync_ShouldReturnEmptyIEnumerableOfTasksComments_WhenCommentsOfTaskDontExist()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var nauahoTask = TestData.TaskFactory.Create(user, project);
        var comments = new List<Comment>()
        {
            CommentFactory.Create(user, task),
            CommentFactory.Create(user, task),
            CommentFactory.Create(user, task, "Test Comment 2"),
            CommentFactory.Create(user, task, "Test Comment 3"),
            CommentFactory.Create(user, task, "Test Comment 4")
        };

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddRangeAsync(task, nauahoTask);
        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();

        // Act
        var result = await _commentService.GetTasksCommentsAsync(nauahoTask.TaskId);

        // Assert
        result.Should().BeEmpty();
        result.Should().BeAssignableTo<IEnumerable<CommentGET>>();
    }
}