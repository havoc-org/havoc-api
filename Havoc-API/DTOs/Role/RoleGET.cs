using Havoc_API.Models;

namespace Havoc_API.DTOs.Role
{
    public class RoleGET
    {
        public int RoleId { get; private set; }
        public RoleType Name { get; private set; }
        public RoleGET(int roleId, RoleType name)
        {
            RoleId = roleId;
            Name = name;
        }
    }
}
