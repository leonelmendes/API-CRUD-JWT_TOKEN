using Api.Model;

namespace Api.Repository.User
{
    public interface IUser_Repository
    {
        Task<string> Create_User(User_Model user);
        Task<string> Update_User(User_Model user);
        Task<string> Delete_User(User_Model user);
    }    
}