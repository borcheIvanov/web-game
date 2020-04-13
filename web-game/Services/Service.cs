using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using web_game.Models;
using web_game.Repositories;

namespace web_game.Services
{
    public class MatchesService : IMatchesService
    {
        private static Match _currentMatch = new Match();

        private static readonly Dictionary<Guid, KeyValuePair<Guid, int>> GeneratedNumbers =
            new Dictionary<Guid, KeyValuePair<Guid, int>>();

   
        private readonly IRepository _gameRepository;

        public MatchesService(IRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public Match GetCurrentMatch()
        {
            if (!ThereIsMatchAvailable())
                _currentMatch = new Match
                {
                    ExpireTime = DateTime.Now.AddSeconds(100)
                };

            return _currentMatch;
        }

        public Game Submit(Guid userId)
        {
            if (!GeneratedNumbers.TryGetValue(userId, out _))
            {
                throw new Exception($"A number was not generated for userId {userId}");
            }
            
            if(GeneratedNumbers[userId].Key != GetCurrentMatch().Id)
            {
                throw new Exception($"Match Expired");
            }
                
            
            var game = new Game
            {
                UserId = userId,
                MatchId = GeneratedNumbers[userId].Key,
                Number = GeneratedNumbers[userId].Value,
                Match = _currentMatch
            };
            
            return _gameRepository.Add(game);
        }

        private static bool ThereIsMatchAvailable()
        {
            return _currentMatch != null && !(_currentMatch?.ExpireTime <= DateTime.Now);
        }

        public int GetRandomNumberForUser(Guid userId)
        {
            if (GeneratedNumbers.TryGetValue(userId, out var keyValuePair))
                if (keyValuePair.Key == GetCurrentMatch().Id)
                    return keyValuePair.Value;


            var randomNumber = new Random().Next(0, 100);
            GeneratedNumbers[userId] = new KeyValuePair<Guid, int>(GetCurrentMatch().Id, randomNumber);
            return randomNumber;
        }
    }
}