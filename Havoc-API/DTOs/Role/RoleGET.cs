namespace Havoc_API.DTOs.Role
{
    public class RoleGET
    {
        public int RoleId { get; private set; }
        public string Name { get; private set; } = null!;
        public RoleGET(int roleId, string name)
        {
            RoleId = roleId;
            Name = name;
        }
    }
}
