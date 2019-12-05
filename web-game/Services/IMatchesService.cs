using System;
using System.Threading.Tasks;
using web_game.Models;

namespace web_game.Services
{
    public interface IMatchesService
    {
        Task<Match> GetCurrentMatch();
        
    }
}