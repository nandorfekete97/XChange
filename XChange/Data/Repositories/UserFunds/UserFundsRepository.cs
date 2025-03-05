using Microsoft.EntityFrameworkCore;
using XChange.Data.context;
using XChange.Data.Entities;

namespace XChange.Data.Repositories.UserFunds;

public class UserFundsRepository : IUserFundsRepository
{
    private XChangeContext _dbContext;

    public UserFundsRepository(XChangeContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserFundEntity> GetUserFundsById(int userFundsId)
    {
        return await _dbContext.UserFunds.FirstOrDefaultAsync(userFunds => userFunds.Id == userFundsId);
    }

    public async Task<List<UserFundEntity>> GetUserFundsByUserId(int userId)
    {
        return await _dbContext.UserFunds.Where(userFunds => userFunds.UserId == userId).ToListAsync();
    }

    public async Task CreateUserFunds(UserFundEntity userFund)
    {
        _dbContext.UserFunds.AddAsync(userFund);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateUserFunds(UserFundEntity userFund)
    {
        _dbContext.UserFunds.Update(userFund);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteUserFundsById(int userFundsId)
    {
        var userFundsToDelete =
            _dbContext.UserFunds.FirstOrDefaultAsync(userFunds => userFunds.Id == userFundsId);

        if (userFundsToDelete is not null)
        {
            _dbContext.Remove(userFundsToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
}