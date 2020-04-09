using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using web_game.Models;
using web_game.Services;

namespace web_game.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class GameController : ControllerBase
    {
        private readonly IMatchesService _matchesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameController(IMatchesService matchesService, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _matchesService = matchesService;
        }

        [SwaggerResponse(StatusCodes.Status401Unauthorized, null, Description = "If user is unauthorized")]
        [SwaggerResponse(StatusCodes.Status200OK, typeof(Match), Description = "Returns the current match")]
        [HttpGet("match")]
        public IActionResult GetCurrentMatch()
        {
            var match = _matchesService.GetCurrentMatch();
            return Ok(match);
        }

        [SwaggerResponse(StatusCodes.Status200OK, typeof(int), Description = "Returns random number")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, null, Description = "If the user is unauthorized")]
        [HttpGet("getNumber")]
        public async Task<IActionResult> GetRandomNumber()
        {
            var user = await GetCurrentUserAsync();
            var randomNumber = _matchesService.GetRandomNumberForUser(Guid.Parse(user.Id));
            return Ok(new {randomNumber});
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}