using Microsoft.AspNetCore.Identity;
using XChange.Data.Entities;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.User;
using XChange.Data.Repositories.UserFunds;
using XChange.Models;

namespace XChange.Services;

public class UserService(
    IUserRepository userRepository,
    IUserFundsRepository userFundsRepository,
    ICurrencyRepository currencyRepository)
{
    private ICurrencyRepository _currencyRepository = currencyRepository;

    public async Task<UserModel> GetUserById(int id)
    {
        if (id == 0)
        {
            throw new ArgumentNullException("Invalid Id, no user found.");
        }
        
        var user = await userRepository.GetUserById(id);

        var userFundEntities = await userFundsRepository.GetUserFundsByUserId(user.Id);

        List<int> currencyIds = userFundEntities.Select(userFund => userFund.CurrencyId).ToList();

        var currencies = await currencyRepository.GetByIds(currencyIds);

        List<CurrencyModel> currencyModels = ConvertCurrencyEntitiesToModels(currencies);
        
        List<UserFundModel> userFundModels = new List<UserFundModel>();
        foreach (var userFundEntity in userFundEntities)
        {
            CurrencyModel currencyModel = currencyModels.First(model => model.Id == userFundEntity.CurrencyId);
            UserFundModel userFundModel = ConvertUserFundEntityToModel(userFundEntity, currencyModel);
            userFundModels.Add(userFundModel);
        }

        return new UserModel(user.Id, user.Name, userFundModels);
    }

    private UserFundModel ConvertUserFundEntityToModel(UserFundEntity userFundEntity, CurrencyModel currencyModel)
    {
        return new UserFundModel(userFundEntity.Id, currencyModel, userFundEntity.Pending, userFundEntity.Disposable);
    }

    private List<CurrencyModel> ConvertCurrencyEntitiesToModels(List<CurrencyEntity> currencyEntities)
    {
        return currencyEntities.Select(currencyEntity => 
            new CurrencyModel(
                currencyEntity.Id, 
                currencyEntity.Name, 
                currencyEntity.ShortName)
        ).ToList();
    }
}