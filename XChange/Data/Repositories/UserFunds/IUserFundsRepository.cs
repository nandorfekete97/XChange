using XChange.Data.Entities;

namespace XChange.Data.Repositories.UserFunds;

public interface IUserFundsRepository
{
    Task<UserFundEntity> GetUserFundsById(int userFundsId);
    Task<List<UserFundEntity>> GetUserFundsByUserId(int userId);
    Task CreateUserFunds(UserFundEntity userFund);
    Task UpdateUserFunds(UserFundEntity userFund);
    Task<bool> DeleteUserFundsById(int userFundsId);
}