using System.Collections.Generic;
using Navigator.Models.Abstract;

namespace Navigator.Dal.Abstract
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
        /// <summary>
        /// Changing user's old password with new one
        /// </summary>
        /// <param name="userId">ID number for user</param>
        /// <param name="oldPassword">User's old password</param>
        /// <param name="newPasswrod">User's new password</param>
        /// <returns>if condition is successful then it returns true which means that the action is completed else it returns false</returns>
        bool ChangeUserPassword(int userId, string oldPassword, string newPasswrod);
        /// <summary>
        /// Generate list of users
        /// </summary>
        /// <returns>list of users</returns>
        List<IUser> GetAll();
        /// <summary>
        /// Get user for given id
        /// </summary>
        /// <param name="userId">ID number of user</param>
        /// <returns>object of user</returns>
        IUser Get(int userId);
        /// <summary>
        /// Create a new user for given role, username, password, full name, phone and email address
        /// </summary>
        /// <param name="roleId">ID number of role</param>
        /// <param name="userName">User's username</param>
        /// <param name="password">User's password</param>
        /// <param name="fullName">User's full name</param>
        /// <param name="phone">User's phone number</param>
        /// <param name="email">User's email address</param>
        /// <returns>if condition is successful then it returns true which means that the action is completed else it returns false</returns>
        bool Insert(int roleId, string userName, string password, string fullName, string phone, string email);
        /// <summary>
        /// Update existing user for given id with new phone number or full name or email address or new status
        /// </summary>
        /// <param name="userId">ID number of user</param>
        /// <param name="userName">User's username</param>
        /// <param name="fullName">User's full name</param>
        /// <param name="phone">User's phone number</param>
        /// <param name="email">User's email address</param>
        /// <param name="active">User's status(active or inactive)</param>
        /// <returns>if condition is successful then it returns true which means that the action is completed else it returns false</returns>
        bool Update(int userId, int roleId,string userName, string fullName, string phone, string email, bool active);
        /// <summary>
        ///  Create password token for reset old password
        /// </summary>
        /// <param name="usermail">User's email address</param>
        /// <returns>mail for user with the token</returns>
        bool CreateResetPasswordToken(string usermail);
        /// <summary>
        /// Resetting user's old password with new one
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="token">User's token</param>
        /// <param name="password">User's password</param>
        /// <returns>if condition is successful then it returns true which means that the action is completed else it returns false</returns>
        bool ResetPassword(string email, string token, string password);
    }
}
