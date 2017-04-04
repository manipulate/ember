using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asterion.Out;
using MySql.Data.MySqlClient;

namespace Ember
{
    class Database
    {
        private MySqlConnection conn;

        public Database(string host)
        {
            try
            {
                conn = new MySqlConnection("server=127.0.0.1;uid=root;pwd=ascent;database=ember;");
                conn.Open();
            }
            catch (MySqlException e)
            {
                Logger.WriteOutput(e.Message, Logger.LogLevel.Error);
            }
        }

        public bool VerifyUser(string name)
        {
            bool value = false;

            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM penguins WHERE username = @username";
                cmd.Parameters.AddWithValue("@username", name);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close();
                    value = true;
                }
                else
                {
                    reader.Close();
                    value = false;
                }
            }
            catch (MySqlException e)
            {
                Logger.WriteOutput(e.Message, Logger.LogLevel.Error);
            }

            return value;
        }

        public bool VerifyUserById(int id)
        {
            bool value = false;

            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM penguins WHERE id = @userid";
                cmd.Parameters.AddWithValue("@userid", id);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close();
                    value = true;
                }
                else
                {
                    reader.Close();
                    value = false;
                }
            }
            catch (MySqlException e)
            {
                Logger.WriteOutput(e.Message, Logger.LogLevel.Error);
            }

            return value;
        }

        public int GetPlayerID(string name)
        {
            string value = "";

            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM penguins WHERE username = @username";
                cmd.Parameters.AddWithValue("@username", name);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    value = reader["id"].ToString();
                }
                cmd.Dispose();
                reader.Close();

            }
            catch (MySqlException e)
            {
                Logger.WriteOutput(e.Message, Logger.LogLevel.Error);
            }

            return Convert.ToInt32(value);
        }

        public string Get(int id, string row)
        {
            string value = "";

            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM penguins WHERE id = @userid";
                cmd.Parameters.AddWithValue("@userid", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                

                while (reader.Read())
                {
                    value = reader[row].ToString();
                }
                cmd.Dispose();
                reader.Close();
            }
            catch (MySqlException e)
            {
                Logger.WriteOutput(e.Message, Logger.LogLevel.Error);
            }

            return value;
        }
        public void Update(int id, string row, string value)
        {
            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE penguins SET " + row + " = '" + value + "' WHERE id = @userid";
                cmd.Parameters.AddWithValue("@userid", id);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (MySqlException e)
            {
                Logger.WriteOutput(e.Message, Logger.LogLevel.Error);
            }
        }
    }
}
