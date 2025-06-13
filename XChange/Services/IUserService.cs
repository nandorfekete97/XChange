using XChange.Data.Entities;
using XChange.Models;

namespace XChange.Services
{
    public interface IUserService
    {
        Task<UserModel> GetById(int id);
        public Task CreateUser(UserModel userModel);
        public Task UpdateUser(UserModel userModel);
        public Task DeleteUser(int id);        
        UserFundModel ConvertUserFundEntityToModel(UserFundEntity userFundEntity, CurrencyModel currencyModel);
        List<CurrencyModel> ConvertCurrencyEntitiesToModels(List<CurrencyEntity> currencyEntities);
    }
}