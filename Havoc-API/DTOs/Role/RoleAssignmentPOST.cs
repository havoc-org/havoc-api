using Havoc_API.Models;

namespace Havoc_API.DTOs.Role
{
    public class RoleAssignmentPOST
    {

        public RoleType Name { get; private set; }
        public RoleAssignmentPOST(int roleId, RoleType name)
        {
            Name = name;
        }
    }
}
