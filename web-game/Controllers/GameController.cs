using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using web_game.Models;
using web_game.Services;
using web_game.Identity;

namespace web_game.Controllers {
    [Route ("api")]
    [ApiController]
    [Authorize]
    public class GameController : ControllerBase {
        private readonly IService _service;
        
        public GameController (IService service) {
            _service = service;
        }

        [SwaggerResponse (StatusCodes.Status401Unauthorized, null, Description = "If user is unauthorized")]
        [SwaggerResponse (StatusCodes.Status200OK, typeof (Match), Description = "Returns the current match")]
        [HttpGet ("match")]
        public IActionResult GetCurrentMatch () {
            var match = _service.GetCurrentMatch ();
            return Ok (match);
        }

        [SwaggerResponse (StatusCodes.Status200OK, typeof (int), Description = "Returns random number")]
        [SwaggerResponse (StatusCodes.Status401Unauthorized, null, Description = "If the user is unauthorized")]
        [HttpGet ("getNumber")]
        public IActionResult GetRandomNumber () {            
            var randomNumber = _service.GetRandomNumberForUser (User.Identity.GetEmail());
            return Ok (new { randomNumber });
        }

        [SwaggerResponse (StatusCodes.Status200OK, typeof (int), Description = "User submits the generated number to the match")]
        [SwaggerResponse (StatusCodes.Status401Unauthorized, null, Description = "If the user is unauthorized")]
        [HttpGet("submit")]
        public IActionResult Submit() {
            var submittedGame = _service.Submit(User.Identity.GetEmail(), User.Identity.GetName());
            return Ok(submittedGame);
        }

        [AllowAnonymous]
        [SwaggerResponse (StatusCodes.Status200OK, typeof (List<Game>), Description = "Returns last winners")]
        [HttpGet ("winners")]
        public ActionResult<IEnumerable<Game>> GetLastWinners () {
            return Ok(_service.GetLastWinners ());
        }

    }
}