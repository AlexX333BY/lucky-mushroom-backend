using LuckyMushroom.Models;

namespace LuckyMushroom.DataTransferObjects
{
    public class RoleDto
    {
        public RoleDto(Role role)
        {
            RoleAlias = role?.RoleAlias;
            RoleName = role?.RoleName;
        }

        public string RoleAlias { get; set; }
        public string RoleName { get; set; }
    }
}
