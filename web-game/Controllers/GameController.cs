using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web_game.Models;
using web_game.Services;

namespace web_game.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IMatchesService _matchesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameController(IMatchesService matchesService, UserManager<ApplicationUser> userManager,
            IGameService gameService)
        {
            _userManager = userManager;
            _gameService = gameService;
            _matchesService = matchesService;
        }

        [HttpGet("api/game/currentMatch")]
        public async Task<IActionResult> GetCurrentMatch()
        {
            var match = await _matchesService.GetCurrentMatch();
            return Ok(match);
        }

        [HttpGet]
        public IActionResult GetRandomNumber()
        {
            var randomNumber = new Random().Next(0, 100);
            return Ok(new {randomNumber});
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] SubmitViewModel submitted)
        {
            var user = await GetCurrentUserAsync();

            if (await _gameService.HasUserSubmitted(user.Id))
                return BadRequest();


            var match = await _matchesService.GetCurrentMatch();

            var game = new Game
            {
                MatchId = match.Id,
                UserId = user.Id,
                Number = submitted.Number
            };

            _gameService.SubmitGame(game);
            return Ok();
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}