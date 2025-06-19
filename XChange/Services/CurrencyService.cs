using System.Collections.Immutable;
using XChange.Data.Entities;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.CurrencyRate;
using XChange.Models;

namespace XChange.Services;

public class CurrencyService : ICurrencyService
{
    private ICurrencyRepository _currencyRepository;
    private ICurrencyRateRepository _currencyRateRepository;

    public CurrencyService(ICurrencyRepository currencyRepository, ICurrencyRateRepository currencyRateRepository)
    {
        _currencyRepository = currencyRepository;
        _currencyRateRepository = currencyRateRepository;
    }

    public async Task<List<CurrencyRateModel>> GetLastCurrencyRatesByCurrencyIds(List<int> currencyIds)
    {
        // hogyha ures az id-k listaja, mi is ures listat adunk vissza
        if (currencyIds.Count == 0)
        {
            return new List<CurrencyRateModel>();
        }
        
        // lekerjuk a currency entityket id-k alapjan, es szinten ures listat adunk vissza, ha nem kapunk vissza currencyt
        List<CurrencyEntity> currencyEntities = await _currencyRepository.GetByIds(currencyIds);
        
        if (currencyEntities.Count == 0)
        {
            return new List<CurrencyRateModel>();
        }

        // kivesszuk a valtasban resztvevo currencyk id-jait egy listaba 
        List<int> validCurrencyIds = currencyEntities.Select(entity => entity.Id).ToList();
        
        List<CurrencyRateModel> currencyRateModels = new List<CurrencyRateModel>();
        
        // egy dictionarybe kiszedjuk a valtasban resztvevo currency id-ket es a hozzajuk tartozo legfrissebb currency rate-t
        Dictionary<int, CurrencyRateEntity> currencyIdToMostRecentCurrencyRates = await _currencyRateRepository.GetLastCurrencyRateByCurrencyIds(validCurrencyIds);
        
        foreach (var currencyEntity in currencyEntities)
        {
            // a currencyEntities a valtasban resztvevo 2 currencyt tartalmazza
            // beazonositjuk a jelenleg iteralt currencyrate entityt es ezt atkonvertaljuk model-re, mert modelt kuldunk a frontendre
            CurrencyRateEntity mostRecentCurrencyRateEntity = currencyIdToMostRecentCurrencyRates[currencyEntity.Id];

            CurrencyModel currencyModel = ConvertCurrencyEntityToModel(currencyEntity);

            CurrencyRateModel currencyRateModel = ConvertCurrencyRateEntityToModel(mostRecentCurrencyRateEntity, currencyModel);

            currencyRateModels.Add(currencyRateModel);
        }

        return currencyRateModels;
    }

    public async Task<CurrencyModel> GetById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("ID must be positive integer.");
        }

        CurrencyEntity currencyEntity = await _currencyRepository.GetById(id);
        
        return ConvertCurrencyEntityToModel(currencyEntity);
    }

    public async Task<List<CurrencyModel>> GetByIds(List<int> ids)
    {
        if (ids.Any(id => id <= 0))
        {
            throw new ArgumentException("All IDs must be positive integers.");
        }

        List<CurrencyEntity> currencyEntities = await _currencyRepository.GetByIds(ids);

        List<CurrencyModel> currencyModels = new List<CurrencyModel>();

        foreach (CurrencyEntity entity in currencyEntities)
        {
            currencyModels.Add(ConvertCurrencyEntityToModel(entity));
        }

        return currencyModels;
    }

    public async Task<List<CurrencyModel>> GetAll()
    {
        List<CurrencyEntity> currencyEntities = await _currencyRepository.GetAll();

        List<CurrencyModel> currencyModels = new List<CurrencyModel>();

        foreach (CurrencyEntity entity in currencyEntities)
        {
            currencyModels.Add(ConvertCurrencyEntityToModel(entity));
        }

        return currencyModels;
    }

    public async Task Create(CurrencyModel currencyModel)
    {
        if (currencyModel.Id != 0)
        {
            throw new ArgumentException("Currency ID must be null.");
        }

        if (currencyModel.Name == "" || currencyModel.ShortName == "")
        {
            throw new ArgumentException("All properties must be filled out.");
        }

        CurrencyEntity newCurrencyEntity = ConvertCurrencyModelToEntity(currencyModel);

        await _currencyRepository.Create(newCurrencyEntity);
    }

    public async Task DeleteById(int currencyId)
    {
        if (currencyId <= 0)
        {
            throw new ArgumentException("Invalid ID.");
        }

        CurrencyEntity currencyToDelete = await _currencyRepository.GetById(currencyId);

        if (currencyToDelete == null)
        {
            throw new ArgumentException("Currency not found, could not be deleted.");
        }

        await _currencyRepository.DeleteById(currencyId);
    }

    public CurrencyRateModel ConvertCurrencyRateEntityToModel(CurrencyRateEntity currencyRateEntity, CurrencyModel currencyModel)
    {
        return new CurrencyRateModel(currencyRateEntity.Id, currencyModel, currencyRateEntity.Rate);
    }
    
    public CurrencyModel ConvertCurrencyEntityToModel(CurrencyEntity currencyEntity)
    {
        return new CurrencyModel(currencyEntity.Id, currencyEntity.Name, currencyEntity.ShortName);
    }

    public CurrencyEntity ConvertCurrencyModelToEntity(CurrencyModel currencyModel)
    {
        return new CurrencyEntity
        {
            Id = currencyModel.Id,
            ShortName = currencyModel.ShortName,
            Name = currencyModel.Name
        };
    }
}