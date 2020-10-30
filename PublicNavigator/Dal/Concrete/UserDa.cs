using System;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using NLog;
using NpgsqlTypes;
using PublicNavigator.Dal.Abstract;
using PublicNavigator.Helpers;
using PublicNavigator.Models.Abstract;
using PublicNavigator.Models.Concrete;

namespace PublicNavigator.Dal.Concrete
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

        private static IUser CreateUserObject(DataRow dr)
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

        public bool Insert(string userName, string password, string fullName, string phone, string email)
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
                            "INSERT INTO users_web(username, pass, fullname, phone, email, salt) VALUES (:un, :pass, :fn, :phone, :email, :salt);";
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
                    @"SELECT u.* FROM users_web u WHERE u.username ilike :u OR u.email ilike :u;";

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
                cmd.CommandText = @"INSERT INTO recovery_web_users(fk_user, token) values (:u, :t) RETURNING *;";
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
                var result = Mail.SendMail(userObj.Email, ConfigurationManager.AppSettings["mailUser"],
                    "Recovery password", msgBody);
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
                cmd.CommandText = @"SELECT * FROM recovery_web_users WHERE token = :t AND now() < validthrough;";

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

                cmd.CommandText = "SELECT salt FROM users_web WHERE user_id=:u;";
                Db.CreateParameterFunc(cmd, "@u", recoveryObj.UserId, NpgsqlDbType.Integer);

                var salt = Db.ExecuteScalar(cmd);

                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "UPDATE users_web SET pass=:np WHERE user_id=:u;";
                Db.CreateParameterFunc(cmd, "@np", CreatePasswordHash(password, salt), NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@u", recoveryObj.UserId, NpgsqlDbType.Integer);
                int rowsAffected = Db.ExecuteNonQuery(cmd);
                if (rowsAffected == 1)
                {
                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        cmd.Connection.Open();
                    }
                    cmd.CommandText = "DELETE FROM recovery_web_users WHERE fk_user = :u;";
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

        public int CheckCounerDxf(int userId)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText = "SELECT counter_dxf FROM users_web WHERE user_id=:id";
                Db.CreateParameterFunc(cmd, "@id", userId, NpgsqlDbType.Integer);
                var counter = int.Parse(Db.ExecuteScalar(cmd));
                return counter;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateCounterDxf(int userId)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText = "UPDATE users_web SET counter_dxf=counter_dxf-1 WHERE user_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", userId, NpgsqlDbType.Integer);
                int rowsAffected = Db.ExecuteNonQuery(cmd);
                if (rowsAffected == 1)
                {
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