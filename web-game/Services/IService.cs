using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using web_game.Models;

namespace web_game.Services
{
    public interface IService
    {
        Match GetCurrentMatch();
        int GetRandomNumberForUser(string userId);
        Game Submit(string userId, string name);
        IEnumerable<Game> GetLastWinners();
    }
}