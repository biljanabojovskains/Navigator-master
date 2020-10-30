using Api.Models;

namespace Api.Dal.Abstract
{
    public interface IUserDa
    {
        User Insert(string userName, string password, string fullName, string email);
        User ValidateUser(string username, string password);
    }
}
