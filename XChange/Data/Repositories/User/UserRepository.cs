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

    public Task<UserEntity?> GetById(int userId)
    {
        return _dbContext.Users.FirstOrDefaultAsync(userEntity => userEntity.Id == userId);
    }

    public async Task<UserEntity?> GetByFirstName(string firstName)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(userEntity => userEntity.FirstName == firstName);
    }

    public async Task<UserEntity?> GetByFullName(string firstName, string lastName)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(userEntity =>
            userEntity.FirstName == firstName && userEntity.LastName == lastName);
    }

    public async Task Create(UserEntity user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(UserEntity user)
    {
        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteById(int userId)
    {
        var userToDelete = await _dbContext.Users.FirstOrDefaultAsync(userEntity => userEntity.Id == userId);

        if (userToDelete is not null)
        {
            _dbContext.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
}