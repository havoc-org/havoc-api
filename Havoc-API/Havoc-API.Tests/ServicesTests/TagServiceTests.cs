using System.Threading.Tasks;
using FluentAssertions;
using Havoc_API.Data;
using Havoc_API.DTOs.Tag;
using Havoc_API.Models;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Xunit;

namespace Havoc_API.Tests.ServicesTests;
public class TagServiceTests
{
    private readonly IHavocContext _context;
    private readonly ITagService _tagService;
    public TagServiceTests()
    {
        _context = HavocTestContextFactory.GetTestContext();
        _tagService = new TagService(_context);
    }

    [Fact]
    public async void AddTagsToTaskAsync_ShouldAddNewUniqueTags_AndReturnEveryAsList()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var tag3 = TagFactory.Create("Test tag3", "#AAAAAA");
        var tag4 = TagFactory.Create("Test tag4", "#BBBBBB");
        var existingTags = new List<Tag>() { tag3, tag4 };
        foreach (var tag in existingTags)
            task.Tags.Add(tag);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Tags.AddRangeAsync(existingTags);
        await _context.SaveChangesAsync();

        var tagsCount = _context.Tags.Count();

        var tag1 = TagFactory.CreatePost("Test tag1", "#FFFFFF");
        var tag2 = TagFactory.CreatePost("Test tag2", "#000000");
        var newTags = new List<TagPOST>() { tag1, tag2 };
        var tagsToAdd = existingTags
            .Select(t => TagFactory.CreatePost(t.Name, t.ColorHex))
            .Concat(newTags)
            .ToList();

        // Act
        var result = await _tagService
            .AddTagsToTaskAsync(tagsToAdd, task.TaskId, project.ProjectId);

        // Assert
        _context.Tags.Count().Should().Be(tagsCount + newTags.Count());
        result.Should().HaveCount(newTags.Count());
        result.Should().BeAssignableTo<IEnumerable<TagGET>>();

        var GetAndPost = result.Zip(newTags, Tuple.Create);
        foreach (var data in GetAndPost)
        {
            data.Item1.Name.Should().Be(data.Item2.Name);
            data.Item1.ColorHex.Should().Be(data.Item2.ColorHex);
        }
    }

    [Fact]
    public async void GetTagsByTaskIdAsync_ShouldReturnListOfTasksTags()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var tag1 = TagFactory.Create("Test tag1", "#FFFFFF");
        var tag2 = TagFactory.Create("Test tag2", "#000000");
        var tag3 = TagFactory.Create("Test tag3", "#AAAAAA");
        var tag4 = TagFactory.Create("Test tag4", "#BBBBBB");
        var existingTags = new List<Tag>() { tag1, tag2, tag3, tag4 };
        foreach (var tag in existingTags)
            task.Tags.Add(tag);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Tags.AddRangeAsync(existingTags);
        await _context.SaveChangesAsync();

        var tagsCount = _context.Tags.Count();
        //Act
        var result = await _tagService.GetTagsByTaskIdAsync(task.TaskId);

        //Assert
        result.Should().BeAssignableTo<IEnumerable<TagGET>>();
        result.Should().HaveCount(tagsCount).And.HaveCountGreaterThan(0);
        var GetAndPost = result.Zip(existingTags, Tuple.Create);
        foreach (var data in GetAndPost)
        {
            data.Item1.Name.Should().Be(data.Item2.Name);
            data.Item1.ColorHex.Should().Be(data.Item2.ColorHex);
        }
    }

    [Fact]
    public async void DeleteTagFromTaskAsync_ShouldDeletetagAndReturnNumberOfAffectedLines_WhenTagExists()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var tagToDelete = TagFactory.Create("Test tag1", "#FFFFFF");
        var tag2 = TagFactory.Create("Test tag2", "#000000");
        var tag3 = TagFactory.Create("Test tag3", "#AAAAAA");
        var tag4 = TagFactory.Create("Test tag4", "#BBBBBB");
        var existingTags = new List<Tag>() { tagToDelete, tag2, tag3, tag4 };
        foreach (var tag in existingTags)
            task.Tags.Add(tag);

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.Tasks.AddAsync(task);
        await _context.Tags.AddRangeAsync(existingTags);
        await _context.SaveChangesAsync();

        var tagsCount = _context.Tags.Count();

        //Act
        var result = await _tagService.DeleteTagFromTaskAsync(tagToDelete.TagId, task.TaskId);

        //Assert
        result.Should().BePositive();
        _context.Tags.Count().Should().Be(tagsCount);
        task.Tags.FirstOrDefault(t => t.TagId == tagToDelete.TagId).Should().BeNull();
    }
}