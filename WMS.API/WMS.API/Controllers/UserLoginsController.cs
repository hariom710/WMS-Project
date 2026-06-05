using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Domain.Interfaces;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserLoginsController : ControllerBase
    {
        private readonly IUserLoginService _userLoginService;

        public UserLoginsController(IUserLoginService userLoginService)
        {
            _userLoginService = userLoginService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserLogins()
        {
            var logins = await _userLoginService.GetAllAsync();
            return Ok(logins);
        }

        [HttpPost]
        public async Task<IActionResult> PostUserLogin([FromBody] Domain.Models.UserLogin userLogin)
        {
            await _userLoginService.CreateAsync(userLogin);
            return CreatedAtAction(nameof(GetUserLogins), new { id = userLogin.UserId }, userLogin);
        }
    }
}
