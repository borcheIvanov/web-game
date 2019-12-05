using System.Linq;
using System.Threading.Tasks;
using web_game.Models;
using web_game.Repositories;

namespace web_game.Services
{
    public class GameService: IGameService
    {
        private readonly IRepository<Game> _repository;
        private readonly IMatchesService _matchesService;

        public GameService(IRepository<Game> repository, IMatchesService matchesService)
        {
            _repository = repository;
            _matchesService = matchesService;
        }
        
        public async Task<bool> HasUserSubmitted(string userId)
        {
            var currentMatch = await _matchesService.GetCurrentMatch();
            var games = await _repository.FindAsync(x => x.MatchId == currentMatch.Id);
            return games.Any(x => x.UserId == userId);
        }

        public void SubmitGame(Game g)
        {
            _repository.Add(g);
        }
    }
}