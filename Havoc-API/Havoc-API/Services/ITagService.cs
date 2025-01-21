using Havoc_API.DTOs.Tag;

namespace Havoc_API.Services;

public interface ITagService
{
    // Получить теги, связанные с задачей
    Task<List<TagGET>> GetTagsByTaskIdAsync(int taskId);



    // Добавить новый тег к задаче
   Task<int> AddTagToTaskAsync(TagPOST tag, int taskId);


    // Удалить тег из задачи
    Task<int> DeleteTagFromTaskAsync(int tagId, int taskId);
}
