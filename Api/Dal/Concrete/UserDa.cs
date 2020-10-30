using System;
using System.Data;
using System.Security.Cryptography;
using System.Web.Security;
using Api.Dal.Abstract;
using Api.Models;
using Navigator.Dal;
using NLog;
using NpgsqlTypes;

namespace Api.Dal.Concrete
{
    public class UserDa : IUserDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public User Insert(string userName, string password, string fullName, string email)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT * FROM users_web WHERE LOWER(email)=LOWER(:email);";
                Db.CreateParameterFunc(cmd, "@email", email, NpgsqlDbType.Text);

                var dt = Db.ExecuteSelectCommand(cmd);

                if (dt.Rows.Count == 0)
                {
                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        cmd.Connection.Open();
                    }
                    cmd.CommandText = "SELECT * FROM users_web WHERE LOWER(username)=LOWER(:un);";
                    Db.CreateParameterFunc(cmd, "@un", userName, NpgsqlDbType.Text);

                    dt = Db.ExecuteSelectCommand(cmd);

                    if (dt.Rows.Count == 0)
                    {
                        if (cmd.Connection.State != ConnectionState.Open)
                        {
                            cmd.Connection.Open();
                        }

                        var salt = CreateSalt(8);

                        cmd.CommandText =
                            "INSERT INTO users_web(username, pass, fullname, email, salt) VALUES (:un, :pass, :fn, :email, :salt) RETURNING user_id;";
                        Db.CreateParameterFunc(cmd, "@pass", CreatePasswordHash(password, salt), NpgsqlDbType.Text);
                        Db.CreateParameterFunc(cmd, "@fn", fullName, NpgsqlDbType.Text);
                        Db.CreateParameterFunc(cmd, "@salt", salt, NpgsqlDbType.Text);

                        var userId = int.Parse(Db.ExecuteScalar(cmd));
                        if (userId > 0)
                        {
                            User item = new User
                            {
                                UserId = userId,
                                UserName = userName,
                                Email = email,
                                FullName = fullName
                            };
                            return item;
                        }
                        return null;
                    }
                    throw new Exception("Постои такво корисничко име");
                }
                throw new Exception("Постои таков емаил");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public User ValidateUser(string username, string password)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT salt FROM users_web WHERE username=:un;";
                Db.CreateParameterFunc(cmd, "@un", username, NpgsqlDbType.Text);
                var salt = Db.ExecuteScalar(cmd);
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "SELECT u.* FROM users_web u WHERE lower(u.username)=lower(:un) AND u.pass= :p;";
                Db.CreateParameterFunc(cmd, "@p", CreatePasswordHash(password, salt), NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                //throw new NotFoundException(string.Format("Wrong username or password"), ErrorCodes.ErrorCodeItemNotFound);
                return null;
            }
            return CreateUserObject(dt.Rows[0]);
        }
        /// <summary>
        /// Changes password hash.
        /// </summary>
        /// <param name="pwd">The password</param>
        /// <param name="salt">The salt</param>
        /// <returns>SHA1 of the password</returns>
        private static string CreatePasswordHash(string pwd, string salt)
        {
            var saltAndPwd = String.Concat(pwd, salt);
            var hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");

            return hashedPwd;
        }

        private static string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        private static User CreateUserObject(DataRow dr)
        {
            var user = new User
            {
                UserId = int.Parse(dr["user_id"].ToString()),
                UserName = dr["username"].ToString(),
                FullName = dr["fullname"].ToString(),
                Phone = dr["phone"].ToString(),
                Email = dr["email"].ToString(),
            };
            return user;
        }

    }
}