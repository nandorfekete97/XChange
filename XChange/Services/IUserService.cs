using XChange.Data.Entities;
using XChange.Models;

namespace XChange.Services
{
    public interface IUserService
    {
        Task<UserModel> GetUserById(int id);
        UserFundModel ConvertUserFundEntityToModel(UserFundEntity userFundEntity, CurrencyModel currencyModel);
        List<CurrencyModel> ConvertCurrencyEntitiesToModels(List<CurrencyEntity> currencyEntities);
    }
}