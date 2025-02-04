using Havoc_API.DTOs.Tag;
using Havoc_API.Models;

public static class TagFactory
{
    public static Tag Create(string name = "TestTag", string colorHex = "#FFFFFF")
    {
        return new Tag(name, colorHex);
    }

    public static TagPOST CreatePost(string name = "TestTag", string colorHex = "#FFFFFF")
    {
        return new TagPOST()
        {
            Name = name,
            ColorHex = colorHex
        };
    }
}