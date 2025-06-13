using Microsoft.AspNetCore.Mvc;
using XChange.Data.Entities;
using XChange.Services;

namespace XChange.Controllers;

[ApiController]
[Route("[controller]")]
public class ExchangeController : ControllerBase
{
    private IExchangeService _exchangeService;

    public ExchangeController(IExchangeService exchangeService)
    {
        _exchangeService = exchangeService;
    }

    [HttpPost("DoExchange")]
    public async Task<IActionResult> DoExchange(int userId, int sourceCurrencyId, int targetCurrencyId, decimal amount)
    {
        await _exchangeService.DoExchange(userId, sourceCurrencyId, targetCurrencyId, amount);
        return Ok(new { message = "Exchange successful." });
    }
    
    [HttpGet("GetExchangeInfoById")]
    public async Task<IActionResult> GetExchangeInfoById([FromQuery]int id)
    {
        ExchangeInfoEntity exchangeInfoEntity = await _exchangeService.GetById(id);
        return Ok(new { exchangeInfoEntity });
    }
    
    [HttpGet("GetExchangeInfosByIds")]
    public async Task<IActionResult> GetExchangeInfosByIds([FromQuery]List<int> ids)
    {
        List<ExchangeInfoEntity> exchangeInfoEntities = await _exchangeService.GetByIds(ids);
        return Ok(new { exchangeInfoEntities });
    }

    [HttpGet("GetExchangeInfosByUserId")]
    public async Task<IActionResult> GetExchangeInfosByUserId(int userId)
    {
        List<ExchangeInfoEntity> exchangeInfoEntities = await _exchangeService.GetByUserId(userId);
        return Ok(new { exchangeInfoEntities });
    }
    
    [HttpDelete("DeleteExchangeInfo")]
    public async Task<IActionResult> DeleteExchangeInfo([FromQuery]int id)
    {
        await _exchangeService.DeleteById(id);
        return Ok(new { message = "ExchangeInfo deleted."});
    }
}