using Microsoft.AspNetCore.Mvc;
using XChange.Data.Entities;
using XChange.Models;
using XChange.Services;

namespace XChange.Controllers;

[ApiController]
[Route("[controller]")]
public class UserFundsController : ControllerBase
{
    private IUserFundsService _userFundsService;

    public UserFundsController(IUserFundsService userFundsService)
    {
        _userFundsService = userFundsService;
    }

    [HttpGet("GetById")]
    public async Task<IActionResult> GetUserFundsById(int userFundId)
    {
        UserFundModel userFundModel = await _userFundsService.GetById(userFundId);
        return Ok(new { userFundModel });
    }
    
    [HttpGet("GetByUserId")]
    public async Task<IActionResult> GetUserFundsByUserId(int userId)
    {
        List<UserFundModel> userFundModels = await _userFundsService.GetByUserId(userId);
        return Ok(new { userFundModels });
    }

    [HttpGet("CreateUserFunds")]
    public async Task<IActionResult> Create([FromBody] UserFundModel userFundModel)
    {
        await _userFundsService.Create(userFundModel);
        return Ok(new { message = "UserFunds created." });
    }

    [HttpPut("UpdateUserFunds")]
    public async Task<IActionResult> Update(UserFundModel userFundModel)
    {
        await _userFundsService.Update(userFundModel);
        return Ok(new { message = "UserFunds updated." });
    }

    [HttpDelete("DeleteUserFunds")]
    public async Task<IActionResult> DeleteUserFundsById(int userFundsId)
    {
        await _userFundsService.DeleteById(userFundsId);
        return Ok(new { message = "UserFunds deleted."});
    }
}