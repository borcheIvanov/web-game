using System;
using System.Collections.Generic;
using System.Linq;
using web_game.Models;

namespace web_game.Repositories
{
    public class Repository : IRepository
    {
        private static readonly List<Game> PlayedGames = new List<Game>();

        public Game Add(Game entity)
        {
            PlayedGames.Add(entity);
            return entity;
        }

        public IEnumerable<Game> GetAll(Func<Game, bool> predicate)
        {
            return PlayedGames.Where(predicate);
        }

        public IEnumerable<Game> GetLastWinners()
        {
            var matchesPlayed = PlayedGames
                .GroupBy(match => match.MatchId)
                .Select(x => x.Key);

            var winners = matchesPlayed
                .Select(matchGuid => PlayedGames
                                        .OrderByDescending(x => x.Number)
                                        .FirstOrDefault(x => x.MatchId == matchGuid));

            return winners;
        }
    }
}