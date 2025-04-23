using XChange.Data.Entities;

namespace XChange.Data.Repositories.UserFunds;

public interface IUserFundsRepository
{
    Task<UserFundEntity> GetById(int userFundsId);
    Task<List<UserFundEntity>> GetByUserId(int userId);
    Task Create(UserFundEntity userFund);
    Task Update(UserFundEntity userFund);
    Task<bool> DeleteById(int userFundsId);
}