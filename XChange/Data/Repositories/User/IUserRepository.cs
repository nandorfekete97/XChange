using XChange.Data.Entities;

namespace XChange.Data.Repositories.User;

public interface IUserRepository
{
    Task<UserEntity?> GetById(int id);
    Task<UserEntity?> GetByFirstName(string name);
    Task<UserEntity?> GetByFullName(string firstName, string lastName);
    Task Create(UserEntity user);
    Task Update(UserEntity user);
    Task<bool> DeleteById(int id);
}