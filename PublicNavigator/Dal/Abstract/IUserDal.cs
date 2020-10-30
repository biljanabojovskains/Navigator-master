using System.Collections.Generic;
using PublicNavigator.Models.Abstract;

namespace PublicNavigator.Dal.Abstract
{
    public interface IUserDal
    {
        /// <summary>
        /// Validating user with his/her username and password
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="password">User's password</param>
        /// <returns>user object</returns>
        IUser ValidateUser(string username, string password);
        int CheckCounerDxf(int userId);
        bool UpdateCounterDxf(int userId);
        bool Insert(string userName, string password, string fullName, string phone, string email);
        bool CreateResetPasswordToken(string usermail);
        bool ResetPassword(string email, string token, string password);
    }
}