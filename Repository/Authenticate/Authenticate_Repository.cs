using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Model;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Api.Repository.Authenticate
{
    public class Authenticate_Repository : IAuthenticate_Repository
    {
        private readonly IConfiguration _configuration;

        public Authenticate_Repository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Login_ResponseModel> Authenticate_Login(User_Model user)
        {
            string response = await User_Login(user);

            if(response.Contains("again"))
                return null;

            string token = Auth_token(user);

            try
            {
                var connectionBD = await Connection.Connection.ConnectionBD();

                if(connectionBD.State == ConnectionState.Open)
                {
                    var command = new NpgsqlCommand()
                    {
                        Connection = connectionBD,
                        CommandText = ""+

                        "Update tbl_User Set token_login = @token_login Where email = @email Or phone = @phone;"
                    };
                    command.Parameters.AddWithValue("@token_login", token);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@phone", user.Phone);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch(Exception)
            {

            }

            return new Login_ResponseModel { Token = token };
        }

        public async Task<string> User_Data(User_Model user)
        {
            var response = string.Empty;
            var connectionBD = await Connection.Connection.ConnectionBD();

            try
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
                    user.Name = reader.GetString(0);
                    user.Email = reader.GetString(1);
                    user.Phone = reader.GetInt32(2);
                }
                else
                {
                    response = "The Email/Phone number or Password you entered is incorret. Please try again.";
                }
            }
            catch(Exception)
            {

            }

            await connectionBD.CloseAsync();
            return response;
        }

        private string Auth_token(User_Model user)
        {
            var response = string.Empty;
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(7), // time to during you token
                    signingCredentials: credentials);

                response = new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                response = $"Something went wrong {ex}";
            }

            return response;
        }

        private async Task<string> User_Login(User_Model user)
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

                        "Select name_, email, phone From tbl_User Where (email = @email Or phone = @phone) And password_ = @password;"
                    };
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@phone", user.Phone);
                    command.Parameters.AddWithValue("@password", user.Password);

                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if(reader.HasRows && await reader.ReadAsync())
                    {
                        user.Name = reader.GetString(0);
                        user.Email = reader.GetString(1);
                        user.Phone = reader.GetInt32(2);
                        user.Password = string.Empty;
                    }
                    else
                    {
                        response = "The Email/Phone number or Password you entered is incorret. Please try again.";
                    }
                }
            }
            catch (Exception)
            {
                response = "Oops! Something went wrong with the login. Please try again later.";
            }

            await connectionBD.CloseAsync();
            return response;
        }
    }
}