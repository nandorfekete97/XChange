using XChange.Data.Entities;
using XChange.Data.Repositories.UserFunds;
using XChange.Models;

namespace XChange.Services;

public interface IUserFundsService
{
    Task<UserFundModel> GetById(int userFundId);
    Task<List<UserFundModel>> GetByUserId(int userId);
    Task Create(UserFundModel userFundModel);
    Task Update(UserFundModel userFundModel);
    Task DeleteById(int userFundModelId);
}