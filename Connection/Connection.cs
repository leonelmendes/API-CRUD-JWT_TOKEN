using Npgsql;

namespace Api.Connection
{
    public class Connection 
    {
        private static string ServerBD = "localhost";
        private static string PortBD = "5432";
        private static string BD = "crud_database";
        private static string UserBD = "postgres";
        private static string PasswordBD = "leonel.3";

        private static string ConnectionString = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4}", ServerBD, PortBD, UserBD, PasswordBD, BD);

        public async static Task<NpgsqlConnection> ConnectionBD()
        {
            NpgsqlConnection connectionBD = new NpgsqlConnection(ConnectionString);
            try
            {
                await connectionBD.OpenAsync();
            }
            catch (Exception)
            {
                await connectionBD.CloseAsync();
            }

            return connectionBD;
        }
    }
}