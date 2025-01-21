using Havoc_API.Data;
using Havoc_API.DTOs.Tag;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Havoc_API.Services;

public class TagService : ITagService
{
    private readonly IHavocContext _havocContext;

    public TagService(IHavocContext havocContext)
    {
        _havocContext = havocContext;
    }

    // Получить все теги для таски
    public async Task<List<TagGET>> GetTagsByTaskIdAsync(int taskId)
    {
        try
        {
            if (await _havocContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId) == null)
                throw new NotFoundException("Task not found");

            return await _havocContext.Tags
                .Include(tag => tag.Tasks)
                .Where(tag => tag.Tasks.Any(t => t.TaskId == taskId))
                .Select(tag => new TagGET(tag.TagId, tag.Name, tag.ColorHex))
                .ToListAsync();
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
    }

    // Добавить новый тег к таску
    public async Task<int> AddTagToTaskAsync(TagPOST tag, int taskId)
    {
        try
        {
            var task = await _havocContext.Tasks.Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.TaskId == taskId)
                ?? throw new NotFoundException("Task not found");

            if (task.Tags.Any(t => t.Name == tag.Name && t.ColorHex == tag.ColorHex))
                throw new DomainException("Tag with the same name and color already exists in this task");

            var newTag = new Tag(tag.Name, tag.ColorHex);
            task.Tags.Add(newTag);

            await _havocContext.SaveChangesAsync();
            return newTag.TagId;
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
        catch (DbUpdateException e)
        {
            throw new DataAccessException(e.Message);
        }
    }

    // Удалить тег из таски
    public async Task<int> DeleteTagFromTaskAsync(int tagId, int taskId)
    {
        try
        {
            var task = await _havocContext.Tasks.Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.TaskId == taskId)
                ?? throw new NotFoundException("Task not found");

            var tag = task.Tags.FirstOrDefault(t => t.TagId == tagId)
                ?? throw new NotFoundException("Tag not found in the task");

            task.Tags.Remove(tag);
            return await _havocContext.SaveChangesAsync();
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
        catch (DbUpdateException e)
        {
            throw new DataAccessException(e.Message);
        }
    }
}
