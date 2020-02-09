using System;
using System.Linq;
using System.Threading.Tasks;
using web_game.Models;
using web_game.Repositories;

namespace web_game.Services
{
    public class MatchesService : IMatchesService
    {
        private readonly IRepository<Match> _repository;

        public MatchesService(IRepository<Match> repository)
        {
            _repository = repository;
        }

        public async Task<Match> GetCurrentMatch()
        {
            var matches = (await _repository.FindAsync(x => x.ExpireTime < DateTime.Now)).ToList();

            return matches.OrderByDescending(x => x.ExpireTime).FirstOrDefault();
        }
    }
}