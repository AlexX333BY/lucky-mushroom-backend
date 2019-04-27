using System.Collections.Generic;
using LuckyMushroom.Models;

namespace LuckyMushroom.DataTransferObjects
{
    public class UserDto
    {
        public UserDto(User user, bool shouldSetPassword)
        {
            UserId = user?.UserId;
            Role = new RoleDto(user?.Role);
            UserCredentials = new UserCredentialsDto(user?.UserCredentials, shouldSetPassword);
        }

        public int? UserId { get; protected set; }

        public virtual RoleDto Role { get; protected set; }
        public virtual UserCredentialsDto UserCredentials { get; protected set; }
    }
}
