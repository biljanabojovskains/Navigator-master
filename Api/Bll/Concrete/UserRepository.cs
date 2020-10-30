using System;
using System.Text;
using Api.Bll.Abstract;
using Api.Dal.Concrete;
using Api.Models;

namespace Api.Bll.Concrete
{
    public class UserRepository : IUserRepository
    {
        public User Register(string username, string password, string email)
        {
            return new UserDa().Insert(username, password, username, email);
        }


        public User ValidateUser(string credentials)
        {
            var encoding = Encoding.GetEncoding("UTF-8"); //iso-8859-1
            try
            {
                credentials = encoding.GetString(Convert.FromBase64String(credentials));
            }
            catch (Exception e)
            {
                return null;
            }
            var separator = credentials.IndexOf(':');
            var name = credentials.Substring(0, separator);
            var password = credentials.Substring(separator + 1);
            var user = new UserDa().ValidateUser(name, password);
            return user;
        }
    }
}