using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using Navigator.Dal.Abstract;
using Navigator.Helpers;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using NLog;
using NpgsqlTypes;
using System.Configuration;
using Org.BouncyCastle.Crypto.Tls;

namespace Navigator.Dal.Concrete
{
    public class UserDa : IUserDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IUser ValidateUser(string username, string password)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT salt FROM users WHERE (username=:un or email=:un) AND active='true';";
                Db.CreateParameterFunc(cmd, "@un", username, NpgsqlDbType.Text);
                var salt = Db.ExecuteScalar(cmd);
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "SELECT u.*, r.rolename,r.rolename_mk FROM users u INNER JOIN roles r ON u.fk_role = r.role_id WHERE lower(u.username) ilike lower(:un) OR lower(u.email) ilike lower(:un) AND u.pass= :p AND u.active=TRUE;";

         


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
        public bool ChangeUserPassword(int userId, string oldPassword, string newPasswrod)
        {
            int rowsAffected;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText = "SELECT salt FROM users WHERE user_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", userId, NpgsqlDbType.Integer);

                var salt = Db.ExecuteScalar(cmd);
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "UPDATE users SET pass=:np WHERE user_id=:id AND pass=:op;";
                Db.CreateParameterFunc(cmd, "@op", CreatePasswordHash(oldPassword, salt), NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@np", CreatePasswordHash(newPasswrod, salt), NpgsqlDbType.Text);

                rowsAffected = Db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return rowsAffected == 1;
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

        public List<IUser> GetAll()
        {

            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "SELECT u.*, r.rolename,r.rolename_mk FROM users u INNER JOIN roles r ON u.fk_role = r.role_id;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IUser> list = (from DataRow dr in dt.Rows select CreateUserObject(dr)).ToList();

            return list;
        }
    
        public bool Insert(int roleId, string userName, string password, string fullName, string phone, string email)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT * FROM users WHERE LOWER(email)=LOWER(:email);";
                Db.CreateParameterFunc(cmd, "@email", email, NpgsqlDbType.Text);

                var dt = Db.ExecuteSelectCommand(cmd);

                if (dt.Rows.Count == 0)
                {
                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        cmd.Connection.Open();
                    }
                    cmd.CommandText = "SELECT * FROM users WHERE LOWER(username)=LOWER(:un);";
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
                            "INSERT INTO users(username, pass, fk_role, fullname, phone, email, salt) VALUES (:un, :pass, :rid, :fn, :phone, :email, :salt);";
                        Db.CreateParameterFunc(cmd, "@rid", roleId, NpgsqlDbType.Integer);
                        Db.CreateParameterFunc(cmd, "@pass", CreatePasswordHash(password, salt), NpgsqlDbType.Text);
                        Db.CreateParameterFunc(cmd, "@fn", fullName, NpgsqlDbType.Text);
                        Db.CreateParameterFunc(cmd, "@phone", phone, NpgsqlDbType.Text);
                        Db.CreateParameterFunc(cmd, "@salt", salt, NpgsqlDbType.Text);

                        var rowsAffected = Db.ExecuteNonQuery(cmd);
                        return rowsAffected == 1;
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

         public bool Update(int userId, int roleId, string userName, string fullName, string phone, string email, bool active)
        {
            int rowsAffected;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT * FROM users WHERE LOWER(email)=LOWER(:email) and user_id!=:id;";
                Db.CreateParameterFunc(cmd, "@email", email, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@id", userId, NpgsqlDbType.Integer);

                var dt = Db.ExecuteSelectCommand(cmd);


                if (dt.Rows.Count == 0)
                {
                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        cmd.Connection.Open();
                    }
                    cmd.CommandText = "SELECT * FROM users WHERE LOWER(username)=LOWER(:un) and user_id!=:id;";
                    Db.CreateParameterFunc(cmd, "@un", userName, NpgsqlDbType.Text);
                    Db.CreateParameterFunc(cmd, "@id", userId, NpgsqlDbType.Integer);

                    if (dt.Rows.Count == 0)
                    {
                        dt = Db.ExecuteSelectCommand(cmd);

                        if (cmd.Connection.State != ConnectionState.Open)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.CommandText =
                    "UPDATE users SET username=:un,fk_role=:rid ,fullname=:fn, phone=:phone, email=:email, active=:active WHERE user_id=:id;";
                        Db.CreateParameterFunc(cmd, "@id", userId, NpgsqlDbType.Integer);
                        Db.CreateParameterFunc(cmd, "@rid", roleId, NpgsqlDbType.Integer);
                        Db.CreateParameterFunc(cmd, "@un", userName, NpgsqlDbType.Text);
                        Db.CreateParameterFunc(cmd, "@fn", fullName, NpgsqlDbType.Text);
                        Db.CreateParameterFunc(cmd, "@phone", phone, NpgsqlDbType.Text);
                        Db.CreateParameterFunc(cmd, "@email", email, NpgsqlDbType.Text);
                        Db.CreateParameterFunc(cmd, "@active", active, NpgsqlDbType.Boolean);

                        rowsAffected = Db.ExecuteNonQuery(cmd);
                        return rowsAffected == 1;


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

        public List<IRole> GetAllRoles()
        {

            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "SELECT role_id,rolename,rolename_mk FROM roles;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IRole> list = (from DataRow dr in dt.Rows select CreateRoleObject(dr)).ToList();

            return list;
        }
      
        private static IRole CreateRoleObject(DataRow dr)
        {
            var role = new Role
            {
                RoleId = int.Parse(dr["role_id"].ToString()),
                RoleName = dr["rolename"].ToString(),
                RoleNameMk = dr["rolename_mk"].ToString()
            };
            return role;
        }

        public IUser Get(int userId)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "SELECT u.*, r.rolename,r.rolename_mk FROM users u INNER JOIN roles r ON u.fk_role = r.role_id WHERE u.user_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", userId, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return CreateUserObject(dt.Rows[0]);
        }
        private static IUser CreateUserObject(DataRow dr)
        {
            var user = new User
            {
                UserId = int.Parse(dr["user_id"].ToString()),
                UserName = dr["username"].ToString(),
                FullName = dr["fullname"].ToString(),
                Phone = dr["phone"].ToString(),
                Email = dr["email"].ToString(),
                Active = bool.Parse(dr["active"].ToString()),
                UserRole = new Role
                {
                    RoleId = int.Parse(dr["fk_role"].ToString()),
                    RoleName = dr["rolename"].ToString(),
                    RoleNameMk = dr["rolename_mk"].ToString()
                }
            };
            return user;
        }
    

        public bool CreateResetPasswordToken(string usermail)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    @"SELECT u.*, r.rolename,r.rolename_mk FROM users u INNER JOIN roles r ON u.fk_role = r.role_id WHERE u.username ilike :u OR u.email ilike :u;";

                Db.CreateParameterFunc(cmd, "@u", usermail, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //throw new Exception(ex.Message);
                return false;
            }
            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }

            var userObj = CreateUserObject(dt.Rows[0]);

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = @"DELETE FROM recovery WHERE fk_user = :u;";
                Db.CreateParameterFunc(cmd, "@u", userObj.UserId, NpgsqlDbType.Integer);
                Db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //throw new Exception(ex.Message);
                return false;
            }
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = @"INSERT INTO recovery(fk_user, token) values (:u, :t) RETURNING *;";
                Db.CreateParameterFunc(cmd, "@u", userObj.UserId, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@t", Guid.NewGuid().ToString(), NpgsqlDbType.Uuid);
                dt = Db.ExecuteSelectCommand(cmd);
                var recoveryObj = CreateRecoveryObject(dt.Rows[0]);
                string msgBody = String.Format(
                    "Кликнете на линкот подоле за да ја ресетирате лозинката <a href=\"{0}\">{1}</a>",
                    HttpUtility.HtmlEncode(
                        "http://" + ConfigurationManager.AppSettings["server"] + ":" +
                        ConfigurationManager.AppSettings["httpPort"] + "/Account/ResetPassword.aspx?token=" +
                        recoveryObj.Token), HttpUtility.HtmlEncode(userObj.UserName + ", кликни тука"));
                var result = Mail.SendMail(userObj.Email, ConfigurationManager.AppSettings["mailUser"],"Recovery password", msgBody);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //throw new Exception(ex.Message);
                return false;
            }
        }

        private IRecovery CreateRecoveryObject(DataRow dr)
        {
            var r = new Recovery
            {
                RecoveryId = int.Parse(dr["recovery_id"].ToString()),
                UserId = int.Parse(dr["fk_user"].ToString()),
                Token = dr["token"].ToString(),
                ValidThrough = DateTime.Parse(dr["validthrough"].ToString())
            };
            return r;
        }

        public bool ResetPassword(string email, string token, string password)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = @"SELECT * FROM recovery WHERE token = :t AND now() < validthrough;";

                Db.CreateParameterFunc(cmd, "@t", token, NpgsqlDbType.Uuid);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }

            var recoveryObj = CreateRecoveryObject(dt.Rows[0]);

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText = "SELECT salt FROM users WHERE user_id=:u;";
                Db.CreateParameterFunc(cmd, "@u", recoveryObj.UserId, NpgsqlDbType.Integer);

                var salt = Db.ExecuteScalar(cmd);

                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "UPDATE users SET pass=:np WHERE user_id=:u;";
                Db.CreateParameterFunc(cmd, "@np", CreatePasswordHash(password, salt), NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@u", recoveryObj.UserId, NpgsqlDbType.Integer);
                int rowsAffected = Db.ExecuteNonQuery(cmd);
                if (rowsAffected == 1)
                {
                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        cmd.Connection.Open();
                    }
                    cmd.CommandText = "DELETE FROM recovery WHERE fk_user = :u;";
                    Db.CreateParameterFunc(cmd, "@u", recoveryObj.UserId, NpgsqlDbType.Integer);
                    Db.ExecuteNonQuery(cmd);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
    }
}