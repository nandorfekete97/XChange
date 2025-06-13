using Microsoft.AspNetCore.Mvc;
using XChange.Data.Entities;
using XChange.Models;
using XChange.Services;

namespace XChange.Controllers;

[ApiController]
[Route("[controller]")]
public class CurrencyController : ControllerBase
{
    private ICurrencyService _currencyService;

    public CurrencyController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    [HttpGet("GetLastCurrencyRatesByCurrencyIds")]
    public async Task<IActionResult> GetLastCurrencyRatesByCurrencyIds([FromQuery]List<int> currencyIds)
    {
        List<CurrencyRateModel> currencyRateEntities = await _currencyService.GetLastCurrencyRatesByCurrencyIds(currencyIds);
        return Ok( currencyRateEntities );
    }

    [HttpGet("GetCurrencyById")]
    public async Task<IActionResult> GetCurrencyById([FromQuery]int id)
    {
        CurrencyModel currencyModel = await _currencyService.GetById(id);
        return Ok(new { currencyModel });
    }
    
    [HttpGet("GetCurrenciesByIds")]
    public async Task<IActionResult> GetCurrenciesByIds(List<int> ids)
    {
        List<CurrencyModel> currencyModels = await _currencyService.GetByIds(ids);
        return Ok(new { currencyModels });
    }
    
    [HttpGet("GetAllCurrencies")]
    public async Task<IActionResult> GetAllCurrencies()
    {
        List<CurrencyModel> currencyModels = await _currencyService.GetAll();
        return Ok(new { currencyModels });
    }

    [HttpPut("CreateCurrency")]
    public async Task<IActionResult> CreateCurrency([FromBody] CurrencyModel currencyModel)
    {
        await _currencyService.Create(currencyModel);
        return Ok(new { message = "Currency created."});
    }
    
    [HttpDelete("DeleteCurrency")]
    public async Task<IActionResult> DeleteCurrency(int id)
    {
        await _currencyService.DeleteById(id);
        return Ok(new { message = "Currency deleted."});
    }
}