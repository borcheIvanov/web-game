using System;
using System.Collections.Generic;
using System.Linq;
using web_game.Models;
using web_game.Repositories;

namespace web_game.Services
{
    public class Service : IService
    {
        private static Match _currentMatch = new Match();

        private static readonly Dictionary<string, KeyValuePair<Guid, int>> GeneratedNumbers =
            new Dictionary<string, KeyValuePair<Guid, int>>();

   
        private readonly IRepository _gameRepository;

        public Service(IRepository gameRepository)
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

        public Game Submit(string userId, string name)
        {
            if (!GeneratedNumbers.TryGetValue(userId, out _))
            {
                throw new Exception($"A number was not generated for userId {userId}");
            }
            
            if(GeneratedNumbers[userId].Key != GetCurrentMatch().Id)
            {
                throw new Exception($"Match Expired");
            }

            if(_gameRepository.GetAll(x => x.UserId == userId && x.MatchId == GetCurrentMatch().Id).Any())
            {
                throw new Exception($"user allready submitted");
            }
                
            
            var game = new Game
            {
                UserId = userId,
                UserName = name,
                MatchId = GeneratedNumbers[userId].Key,
                Number = GeneratedNumbers[userId].Value,
                Match = _currentMatch
            };
            
            return _gameRepository.Add(game);
        }

        public IEnumerable<Game> GetLastWinners()
        {
            return _gameRepository.GetLastWinners();
        }

        private static bool ThereIsMatchAvailable()
        {
            return _currentMatch != null && !(_currentMatch?.ExpireTime <= DateTime.Now);
        }

        public int GetRandomNumberForUser(string userId)
        {
            var randomNumber = new Random().Next(0, 100);

            if (GeneratedNumbers.TryGetValue(userId, out var keyValuePair))
                if (keyValuePair.Key == GetCurrentMatch().Id && randomNumber == keyValuePair.Value)
                    randomNumber = new Random().Next(0, 100);


            GeneratedNumbers[userId] = new KeyValuePair<Guid, int>(GetCurrentMatch().Id, randomNumber);
            return randomNumber;
        }
    }
}