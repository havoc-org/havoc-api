using System.Reflection.Metadata.Ecma335;
using Havoc_API.DTOs.Task;

namespace Havoc_API.DTOs.Tag;

public class TagGET
{
    public int TagId { get; set; }

    public string Name { get; set; } = null!;

    public string ColorHex { get; set; } = null!;

    public virtual ICollection<TaskGET> Tasks { get; set; } = new List<TaskGET>();

    public TagGET(int tagId, string name, string colorHex)
    {
        TagId = tagId;
        Name = name;
        ColorHex = colorHex;
    }
}