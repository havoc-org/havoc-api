using Havoc_API.Data;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Havoc_API.DTOs.Assignment;
using FluentAssertions;
using Xunit;
using Havoc_API.Exceptions;
using Havoc_API.Models;

namespace Havoc_API.Tests.ServicesTests;

public class AssignmentServiceTests
{
    private readonly IHavocContext _context;
    private readonly IAssignmentService _assignmentService;

    public AssignmentServiceTests()
    {
        _context = HavocTestContextFactory.GetTestContext();
        _assignmentService = new AssignmentService(_context);
    }

    [Fact]
    public async void AddAssignmentAsync_ShouldAddAssignment_WhenAssignmentDoesntExist()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        int assignmentCount = _context.Assignments.Count();
        var assignment = AssignmentFactory.CreatePost(user.UserId);
        // Act
        var result = await _assignmentService.AddAssignmentAsync(
            assignment,
            task.TaskId
        );

        // Assert
        result.Should().BeTrue();
        _context.Assignments.Count().Should().Be(assignmentCount + 1);
        _context.Assignments.Where(a => a.TaskId == task.TaskId && a.UserId == user.UserId)
            .Should().HaveCount(1);
    }

    [Fact]
    public async void AddAssignmentAsync_ShouldNotAddAssignment_WhenAssignmentExists()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var assignment = AssignmentFactory.Create(user, task);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Assignments.AddAsync(assignment);
        await _context.SaveChangesAsync();

        int assignmentCount = _context.Assignments.Count();

        // Act
        var action = async () => await _assignmentService.AddAssignmentAsync(
            AssignmentFactory.CreatePost(user.UserId, assignment.Description),
            task.TaskId
        );

        // Assert
        await action.Should().ThrowAsync<DomainException>()
            .WithMessage($"This assignment already exists userID: {assignment.UserId} projectID: {assignment.TaskId}");
        _context.Assignments.Count().Should().Be(assignmentCount);
        _context.Assignments.Where(a => a.TaskId == task.TaskId && a.UserId == user.UserId)
            .Should().HaveCount(1);
    }

    [Fact]
    public async void AddManyAssignmentsAsync_ShouldAddOnlyNewAssignments_WhenSomeAssignmentsExist()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var assignments = new[] {
            AssignmentFactory.Create(user, task)
        };

        var user2 = UserFactory.Create("user2@us.com");

        await _context.Users.AddRangeAsync(user, user2);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Assignments.AddRangeAsync(assignments);
        await _context.SaveChangesAsync();

        int assignmentCount = _context.Assignments.Count();

        var newAssignments = new List<AssignmentPOST>
        {
            AssignmentFactory.CreatePost(user2.UserId, "TestDescription2")
        };

        var assignmentsToAdd = new List<AssignmentPOST> {
            AssignmentFactory.CreatePost(user.UserId, "TestDescription1"),
        };

        assignmentsToAdd.AddRange(newAssignments);

        // Act,
        var result = await _assignmentService.AddManyAssignmentsAsync(
            assignmentsToAdd,
            task.TaskId,
            project.ProjectId
        );

        // Assert
        result.Should().BeAssignableTo<IEnumerable<AssignmentGET>>();
        var resultSet = result.Zip(newAssignments, Tuple.Create);
        foreach (var GetAndPost in resultSet)
        {
            GetAndPost.Item1.Description.Should().Be(GetAndPost.Item2.Description);
            GetAndPost.Item1.User.UserId.Should().Be(GetAndPost.Item1.User.UserId);
        }
        _context.Assignments.Count().Should().Be(assignmentCount + newAssignments.Count());
    }


    [Fact]
    public async void DeleteAssignmentAsync_ShouldDeleteAssignmentAndReturnNumberOfAffectedRows_WhenAssignmentExists()
    {
        // Arrange
        var user = UserFactory.Create();
        var user2 = UserFactory.Create("user2@gmail.user");
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);

        var assignmentToDelete = AssignmentFactory.Create(user2, task);

        var assignments = new[]
        {
            assignmentToDelete,
            AssignmentFactory.Create(user, task)
        };

        await _context.Users.AddRangeAsync(user, user2);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Assignments.AddRangeAsync(assignments);
        await _context.SaveChangesAsync();

        int assignmentCount = _context.Assignments.Count();
        // Act
        var result = await _assignmentService.DeleteAssignmentAsync(
            task.TaskId,
            assignmentToDelete.UserId,
            project.ProjectId
        );

        // Assert
        result.Should().BePositive();
        _context.Assignments.Count().Should().Be(assignmentCount - 1);
        _context.Assignments
            .FirstOrDefault(a => a.UserId == assignmentToDelete.UserId && a.TaskId == assignmentToDelete.TaskId)
            .Should().BeNull();
    }

    [Fact]
    public async void DeleteManyAssignmentsAsync_ShouldDeleteOnlyThoseAssignementsThatExist_AndReturnNumberOfAffectedLines()
    {
        // Arrange
        var user = UserFactory.Create();
        var user2 = UserFactory.Create("user2@gmail.user");
        var user3 = UserFactory.Create("user3@gmail.user");
        var user4 = UserFactory.Create("user4@gmail.user");
        var user5 = UserFactory.Create("user5@gmail.user");
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var task2 = TestData.TaskFactory.Create(user2, project);

        var assignmentsToDelete = new List<Assignment>(){
            AssignmentFactory.Create(user2, task),
            AssignmentFactory.Create(user4, task),
        };

        var assignments = new List<Assignment>(){
                AssignmentFactory.Create(user, task2),
                AssignmentFactory.Create(user3, task2),
                AssignmentFactory.Create(user5, task2)
            };

        assignments.AddRange(assignmentsToDelete);

        await _context.Users.AddRangeAsync(user, user2);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddRangeAsync(task, task2);
        await _context.Assignments.AddRangeAsync(assignments);
        await _context.SaveChangesAsync();

        int assignmentCount = _context.Assignments.Count();
        var assignmentsDelete = assignments.Select(a => AssignmentFactory.CreateDelete(a.UserId)).ToList();

        // Act
        var result = await _assignmentService.DeleteManyAssignmentsAsync(
            assignmentsDelete,
            task.TaskId,
            project.ProjectId
        );

        // Assert
        result.Should().BePositive();
        _context.Assignments.Count().Should().Be(assignmentCount - assignmentsToDelete.Count());
        foreach (var assignment in assignmentsToDelete)
            _context.Assignments
                .FirstOrDefault(a => a.UserId == assignment.UserId && a.TaskId == assignment.TaskId)
                .Should().BeNull();
    }
}
