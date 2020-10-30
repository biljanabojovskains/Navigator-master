using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Dal
{
    class Db
    {
        // execute an update, delete, or insert command 
        // and return the number of affected rows
        public static int ExecuteNonQuery(NpgsqlCommand command, bool closeConn = true)
        {
            // The number of affected rows 
            int affectedRows = -1;
            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Execute the command and get the number of affected rows
                affectedRows = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw ex;
            }
            finally
            {
                if (closeConn)
                {
                    // Close the connection
                    command.Connection.Close();
                }
            }
            // return the number of affected rows
            return affectedRows;
        }

        // execute a select command and return a single result as a string
        public static string ExecuteScalar(NpgsqlCommand command, bool closeConn = true)
        {
            // The value to be returned 
            string value = "";
            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the connection of the command
                //command.Connection.Open();
                // Execute the command and get the number of affected rows
                //value = command.ExecuteScalar().ToString();
                var c = command.ExecuteScalar();
                if (c != null)
                {
                    value = c.ToString();
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw ex;
            }
            finally
            {
                if (closeConn)
                {
                    // Close the connection
                    command.Connection.Close();
                }
            }
            // return the result
            return value;
        }

        /// <summary>
        /// Method that executes select command against database.
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <returns>DataTable with results or null if exception has occured</returns>
        /// <exception cref="Exception">Throws Exception if something went wrong</exception>
        public static DataTable ExecuteSelectCommand(NpgsqlCommand command)
        {
            // The DataTable to be returned 
            DataTable table;

            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the data connection 

                //iskomentirano e zosto ne progashe so nego
                //command.Connection.Open(); 

                // Execute the command and save the results in a DataTable
                DbDataReader reader = command.ExecuteReader();
                table = new DataTable("myData");
                table.Load(reader);

                // Close the reader 
                reader.Close();
            }
            catch (Exception ex)
            {

                //logger.Error(ex.Message, ex);
                throw;
            }
            finally
            {
                // Close the connection
                command.Connection.Close();
            }
            return table;
        }

        // creates and prepares a new DbCommand object on a new connection
        public static NpgsqlCommand CreateCommand()
        {
            // Obtain the database connection string
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
           // string connectionString = "Server=C; Port = 5434; Database=navigator_centar; User ID=postgres; Password=Alfa321@; Pooling=false;";
          
            // string connectionString = "Server=beta; Port = 5432; Database=geocentar; User ID=postgres; Password=Beta321@; Pooling=false;";

            // Obtain a database specific connection object
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            // Create a database specific command object
            NpgsqlCommand comm = conn.CreateCommand();
            // Set the command type to stored procedure
            comm.CommandType = CommandType.Text;
            // Return the initialized command object
            return comm;
        }

        public static void CreateParameterFunc(NpgsqlCommand cmd, string name, ParameterDirection direction, object value, NpgsqlDbType type)
        {
            NpgsqlParameter param = cmd.CreateParameter();

            param.ParameterName = name;
            param.Direction = direction;
            param.NpgsqlValue = value;
            param.NpgsqlDbType = type;
            cmd.Parameters.Add(param);
        }

        public static void CreateParameterFunc(NpgsqlCommand cmd, string name, object value, NpgsqlDbType type)
        {
            NpgsqlParameter param = cmd.CreateParameter();

            param.ParameterName = name;
            param.Direction = ParameterDirection.Input;
            param.NpgsqlValue = value;
            param.NpgsqlDbType = type;
            cmd.Parameters.Add(param);
        }
    }
}
