using LuckyMushroom.Models;

namespace LuckyMushroom.DataTransferObjects
{
    public class UserCredentialsDto
    {
        public UserCredentialsDto(UserCredentials creds, bool shouldSetPassword = false)
        {
            UserMail = creds?.UserMail;
            UserPasswordHash = shouldSetPassword && (creds != null) ? creds.UserPasswordHash : null;
        }
        public string UserMail { get; set; }
        public string UserPasswordHash { get; set; }
    }
}
