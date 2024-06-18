using System.Data;
using Api.Model;
using Npgsql;

namespace Api.Repository.User
{
    public class User_Repository : IUser_Repository
    {
        public async Task<string> Create_User(User_Model user)
        {
            var response = string.Empty;
            var connectionBD = await Connection.Connection.ConnectionBD();

            try
            {
                if(await User_Exists(user))
                {
                    response = "oops! Email or phone number already registered. Please try loggin in or using different information.";
                }
                else
                {
                    if(connectionBD.State == ConnectionState.Open)
                    {
                        var command = new NpgsqlCommand()
                        {
                            Connection = connectionBD,
                            CommandText = ""+

                            "Insert Into tbl_User (name_, email, phone, password_) Values (@name, @email, @phone, @password);"
                        };
                        command.Parameters.AddWithValue("@name", user.Name);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@phone", user.Phone);
                        command.Parameters.AddWithValue("@password", user.Password);

                        await command.ExecuteNonQueryAsync();

                        response = $"Regustration completed! {user.Name} Welcome to. Have fun and make the most of it!";
                    }
                    else
                    {
                        response = "Oops! Something went wrong with you registration. please try again later.";
                    }
                }
            }
            catch (Exception)
            {
                response = "Oops! Something went wrong with you registration. please try again later.";
            }

            await connectionBD.CloseAsync();
            return response;
        }

        private async Task<bool> User_Exists(User_Model user)
        {
            var response = false;
            var connectionBD = await Connection.Connection.ConnectionBD();

            try
            {
                if(connectionBD.State == ConnectionState.Open)
                {
                    var command = new NpgsqlCommand()
                    {
                        Connection = connectionBD,
                        CommandText = ""+

                        "Select name_, email, phone From tbl_User Where email = @email Or phone = @phone;"
                    };
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@phone", user.Phone);

                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if(reader.HasRows && await reader.ReadAsync())
                    {
                        response = true;
                    }
                    else
                    {
                        response = false;
                    }
                }
            }
            catch (Exception)
            {
                response = false;
            }
            await connectionBD.CloseAsync();
            return response;
        }

        public async Task<string> Delete_User(User_Model user)
        {
            var response = string.Empty;
            var connectionBD = await Connection.Connection.ConnectionBD();

            try
            {
                if(connectionBD.State == ConnectionState.Open)
                {
                    var command = new NpgsqlCommand()
                    {
                        Connection = connectionBD,
                        CommandText = ""+

                        "Delete From tbl_User Where email = @email Or phone = @phone;"
                    };
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@phone", user.Phone);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if(rowsAffected > 0)
                    {
                        response = "Record deleted sucessfully.";
                    }
                    else
                    {
                        response = "No records were deleted. Check that email or phone number is corret.";
                    }
                }
            }
            catch (Exception)
            {
                response = "Unable to connect to database.";
            } 
            await connectionBD.CloseAsync();
            return response;
        }

        public async Task<string> Update_User(User_Model user)
        {
            var response = string.Empty;
            var connectionBD = await Connection.Connection.ConnectionBD();

            try
            {
                if(connectionBD.State == ConnectionState.Open)
                {
                    var command = new NpgsqlCommand()
                    {
                        Connection = connectionBD,
                        CommandText = ""+

                        "Update tbl_User Set name_ = @name, email = @email, phone = @phone, password_ = @password Where id_ = @id;"
                    };
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@name", user.Name);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@phone", user.Phone);
                    command.Parameters.AddWithValue("@password", user.Password);

                    await command.ExecuteNonQueryAsync();

                    response = "User data has been changed successfully";
                }
            }
            catch (Exception)
            {
                response = "Oops! Something went wrong with the data change. Please try again later.";
            }

            await connectionBD.CloseAsync();
            return response;
        }
    }
}