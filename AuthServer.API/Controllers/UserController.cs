using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UserController : CustomBaseController
    {
        // DI
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            var result = await _userService.CreateUserAsync(createUserDto);

            return ActionResultInstance(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
                var result = await _userService.GetUserByNameAsync(HttpContext.User.Identity?.Name);

                return ActionResultInstance(result);
        }
    }
}