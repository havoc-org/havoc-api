namespace Havoc_API.DTOs.Role
{
    public class RoleAssignmentPOST
    {

        public string Name { get; private set; } = null!;
        public RoleAssignmentPOST(int roleId, string name)
        {
            Name = name;
        }
    }
}
