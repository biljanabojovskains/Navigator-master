using Api.Models;

namespace Api.Bll.Abstract
{
    public interface IUserRepository
    {
        User Register(string username, string password, string email);

        /// <summary>
        /// Validates user.
        /// </summary>
        /// <param name="credentials">The credentials of the user</param>
        /// <returns>User object if valid, null if not valid</returns>
        User ValidateUser(string credentials);
    }
}
