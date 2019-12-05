using System.Threading.Tasks;
using web_game.Models;

namespace web_game.Services
{
    public interface IGameService
    {
        Task<bool> HasUserSubmitted(string userId);

        void SubmitGame(Game g);
    }
}