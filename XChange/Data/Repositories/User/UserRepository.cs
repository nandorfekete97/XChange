using Microsoft.EntityFrameworkCore;
using XChange.Data.context;
using XChange.Data.Entities;

namespace XChange.Data.Repositories.User;

public class UserRepository : IUserRepository
{
    private XChangeContext _dbContext;

    public UserRepository(XChangeContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UserEntity?> GetUserById(int userId)
    {
        return _dbContext.Users.FirstOrDefaultAsync(userEntity => userEntity.Id == userId);
    }

    public async Task<UserEntity?> GetUserByName(string name)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(userEntity => userEntity.Name == name);
    }

    public async Task CreateUser(UserEntity user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateUser(UserEntity user)
    {
        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteUserById(int userId)
    {
        var userToDelete = _dbContext.Users.FirstOrDefaultAsync(userEntity => userEntity.Id == userId);

        if (userToDelete is not null)
        {
            _dbContext.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
}