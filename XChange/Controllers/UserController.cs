using Microsoft.AspNetCore.Mvc;
using XChange.Data.Entities;
using XChange.Models;
using XChange.Services;

namespace XChange.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("GetUserById")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        UserModel userModel = await _userService.GetById(userId);
        return Ok(new { userModel });
    }
}