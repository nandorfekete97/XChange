using XChange.Data.Entities;

namespace XChange.Data.Repositories.User;

public interface IUserRepository
{
    Task<UserEntity?> GetUserById(int id);
    Task<UserEntity?> GetUserByName(string name);
    Task CreateUser(UserEntity user);
    Task UpdateUser(UserEntity user);
    Task<bool> DeleteUserById(int id);
}