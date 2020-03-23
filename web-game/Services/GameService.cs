using System.Linq;
using System.Threading.Tasks;
using web_game.Models;
using web_game.Repositories;

namespace web_game.Services
{
    public class GameService : IGameService
    {
        private readonly IMatchesService _matchesService;
        private readonly IRepository<Game> _repository;

        public GameService(IRepository<Game> repository, IMatchesService matchesService)
        {
            _repository = repository;
            _matchesService = matchesService;
        }

        public async Task<bool> HasUserSubmitted(string userId)
        {
            var currentMatch = _matchesService.GetCurrentMatch();
            var games = await _repository.FindAsync(x => x.MatchId == currentMatch.Id);
            return true;
        }

        public void SubmitGame(Game g)
        {
            _repository.Add(g);
        }
    }
}