using Havoc_API.DTOs.Tag;

namespace Havoc_API.Services;

public interface ITagService
{

    Task<List<TagGET>> GetTagsByTaskIdAsync(int taskId);


    Task<IEnumerable<TagGET>> AddTagsToTaskAsync(IEnumerable<TagPOST> tags, int taskId, int projectId);


    Task<int> DeleteTagFromTaskAsync(int tagId, int taskId);
}
