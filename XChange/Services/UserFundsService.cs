using XChange.Data.Entities;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.UserFunds;
using XChange.Models;

namespace XChange.Services;

public class UserFundsService : IUserFundsService
{
    private IUserFundsRepository _userFundsRepository;
    private ICurrencyRepository _currencyRepository;

    public UserFundsService(IUserFundsRepository userFundsRepository, ICurrencyRepository currencyRepository)
    {
        _userFundsRepository = userFundsRepository;
        _currencyRepository = currencyRepository;
    }

    public async Task<UserFundModel> GetById(int userFundId)
    {
        if (userFundId <= 0)
        {
            throw new ArgumentException("ID must be positive integer.");
        }

        UserFundEntity userFundEntity = await _userFundsRepository.GetById(userFundId);

        CurrencyEntity currencyEntity = await _currencyRepository.GetById(userFundEntity.CurrencyId);

        CurrencyModel currencyModel = ConvertCurrencyEntityToModel(currencyEntity);

        UserFundModel userFundModel = ConvertUserFundEntityToModel(userFundEntity, currencyModel);
        
        return userFundModel;
    }

    public async Task<List<UserFundModel>> GetByUserId(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("ID must be positive integer.");
        }

        List<UserFundEntity> userFundEntities = await _userFundsRepository.GetByUserId(userId);

        List<UserFundModel> userFundModels = new List<UserFundModel>();

        foreach (UserFundEntity entity in userFundEntities)
        {
            CurrencyEntity currencyEntity = await _currencyRepository.GetById(entity.CurrencyId);

            CurrencyModel currencyModel = ConvertCurrencyEntityToModel(currencyEntity);
            
            userFundModels.Add(ConvertUserFundEntityToModel(entity, currencyModel));
        }

        return userFundModels;
    }

    public async Task Create(UserFundModel userFundModel)
    {
        if (userFundModel.Id != 0)
        {
            throw new ArgumentException("UserFund ID must be null.");
        }
        if (userFundModel.Disposable < 0)
        {
            throw new ArgumentException("Disposable cannot be negative integer.");
        }

        if (userFundModel.Pending < 0)
        {
            throw new ArgumentException("Pending cannot be negative integer.");
        }

        if (userFundModel.CurrencyModel == null)
        {
            throw new ArgumentException("Currency not found.");
        }

        CurrencyEntity currencyEntity = await _currencyRepository.GetById(userFundModel.CurrencyModel.Id);

        await _userFundsRepository.Create(ConvertUserFundModelToEntity(userFundModel, currencyEntity.Id));
    }

    public async Task Update(UserFundModel userFundModel)
    {
        UserFundEntity userFundToUpdate = await _userFundsRepository.GetById(userFundModel.Id);

        if (userFundToUpdate == null)
        {
            throw new ArgumentException("UserFund not found.");
        }
        
        if (userFundModel.Disposable < 0)
        {
            throw new ArgumentException("Disposable cannot be negative integer.");
        }

        if (userFundModel.Pending < 0)
        {
            throw new ArgumentException("Pending cannot be negative integer.");
        }

        if (userFundModel.CurrencyModel == null)
        {
            throw new ArgumentException("Currency not found.");
        }
        
        CurrencyEntity currencyEntity = await _currencyRepository.GetById(userFundModel.CurrencyModel.Id);

        await _userFundsRepository.Update(ConvertUserFundModelToEntity(userFundModel, currencyEntity.Id));
    }

    public async Task DeleteById(int userFundModelId)
    {
        if (userFundModelId <= 0)
        {
            throw new ArgumentException("Invalid ID.");
        }

        UserFundEntity userFundToDelete = await _userFundsRepository.GetById(userFundModelId);

        if (userFundToDelete == null)
        {
            throw new ArgumentException("UserFund not found, could not be deleted.");
        }

        await _userFundsRepository.DeleteById(userFundModelId);
    }

    private UserFundModel ConvertUserFundEntityToModel(UserFundEntity userFundEntity, CurrencyModel currencyModel)
    {
        return new UserFundModel(userFundEntity.Id, userFundEntity.UserId, currencyModel, userFundEntity.Pending, userFundEntity.Disposable);
    }
    
    private CurrencyModel ConvertCurrencyEntityToModel(CurrencyEntity currencyEntity)
    {
        return new CurrencyModel(currencyEntity.Id, currencyEntity.Name, currencyEntity.ShortName);
    }

    private UserFundEntity ConvertUserFundModelToEntity(UserFundModel userFundModel, int currencyId)
    {
        return new UserFundEntity
        {
            Id = userFundModel.Id,
            UserId = userFundModel.UserId,
            CurrencyId = currencyId,
            Disposable = userFundModel.Disposable,
            Pending = userFundModel.Pending
        };
    }
}