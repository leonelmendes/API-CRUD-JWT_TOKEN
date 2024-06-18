using Api.Model;

namespace Api.Repository.Authenticate
{
    public interface IAuthenticate_Repository
    {
        Task<Login_ResponseModel> Authenticate_Login(User_Model user);
        Task<string> User_Data(User_Model user);
    }
}