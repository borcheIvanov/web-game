using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using web_game.Models;
using web_game.Repositories;
using web_game.Services;

namespace web_game.tests
{
    public class ServiceTests
    {
        private readonly IRepository _gameRepository;
        private readonly IService _service;

        public ServiceTests()
        {
            _gameRepository = new Repository();
            _service = new Service(_gameRepository);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetCurrentMatch_ShouldCreateMatchInMemory()
        {
            var result = _service.GetCurrentMatch();
            result.Should().BeOfType<Match>();
        }

        [Test]
        public void GetCurrentMatch_Should_BeNotNull()
        {
            var result = _service.GetCurrentMatch();
            result.Should().NotBeNull();
        }

        [Test]
        public void GetCurrentMatch_ShouldCreateNewMatchIfNoCurrentExists()
        {
            var result = _service.GetCurrentMatch();
            result.RemainingTime.Seconds.Should().BeGreaterThan(0);
        }

        [Test]
        public void GetCurrentMatch_ShouldReturnTheSameMatch_IfItIsNotExpired()
        {
            var result1 = _service.GetCurrentMatch();
            result1.RemainingTime.Seconds.Should().BeGreaterThan(0);

            var result2 = _service.GetCurrentMatch();

            result1.RemainingTime.Seconds.Should().BeGreaterThan(0);
            result2.RemainingTime.Seconds.Should().BeGreaterThan(0);
        }

        [Test]
        public void GetCurrentMatch_ShouldReturnDifferentMatch_WhenExpired()
        {
            var result1 = _service.GetCurrentMatch();
            result1.ExpireTime = DateTime.Now;

            var result2 = _service.GetCurrentMatch();

            result1.Should().NotBeSameAs(result2);
            result1.RemainingTime.TotalMilliseconds.Should().BeLessThan(0);
            result2.RemainingTime.TotalMilliseconds.Should().BeGreaterThan(0);
        }

        [Test]
        public void GetRandom_ShouldReturnRandomNumberFrom0To100()
        {
            var userId = "borche@blabla.com";
            var result = _service.GetRandomNumberForUser(userId);

            result.Should().BeGreaterThan(0);
            result.Should().BeLessOrEqualTo(100);
        }

        [Test]
        public void GetRandom_ShouldReturnDifferentNumberForSameMatch()
        {
            var userId = "Bla@bla.com";
            var result = _service.GetRandomNumberForUser(userId);
            var result2 = _service.GetRandomNumberForUser(userId);

            result.Should().NotBe(result2);
        }

        [Test]
        public void GetRandom_ShouldReturnDifferentNumberForSameMatchForDifferentUser()
        {
            var result = _service.GetRandomNumberForUser("userId1");
            var result2 = _service.GetRandomNumberForUser("userId2");

            result.Should().NotBe(result2);
        }

        [Test]
        public void GetRandom_ShouldReturnDifferentNumberIfMatchExpired()
        {
            var userId = "user1Key";
            var match = _service.GetCurrentMatch();

            var result = _service.GetRandomNumberForUser(userId);
            match.ExpireTime = DateTime.Now;
            var result2 = _service.GetRandomNumberForUser(userId);

            result.Should().NotBe(result2);
        }


        [Test]
        public void UserCanSubmitNumberToCurrentGame()
        {
            var userId = "borche.ivanov@abc.com";
            var match = _service.GetCurrentMatch();
            var rand = _service.GetRandomNumberForUser(userId);
            var game = new Game {UserId = userId, Number = rand, MatchId = match.Id, Match = match};
            
            var submittedGame = _service.Submit(userId, "name");
            
            submittedGame.Id.Should().NotBe(game.Id);
            submittedGame.Match.Id.Should().Be(match.Id);
            submittedGame.UserId.Should().Be(userId);
            submittedGame.Number.Should().Be(rand);
        }

        [Test]
        public void ExceptionShouldBeThrown_ThereIsNoGeneratedNumber()
        {
            var userId = "userIdKey";
            var match = _service.GetCurrentMatch();

            try
            {
                _service.Submit(userId, "name");
                Assert.Fail();
            }
            catch (Exception e)
            {
                e.Message.Should().Be($"A number was not generated for userId {userId}");
            }
        }

        [Test]
        public void ExceptionShouldBeThrown_ThereIsNoNumberForTheCurrentMatch()
        {
            var userId = "userKey";
            var match = _service.GetCurrentMatch();
            var rand = _service.GetRandomNumberForUser(userId);

            match.ExpireTime = DateTime.Now;

            try
            {
                _service.Submit(userId, "name");
                Assert.Fail();
            }
            catch (Exception e)
            {
                e.Message.Should().Be("Match Expired");
            }
        }

        [Test]
        public void Test_GetLastWinners()
        {
            var user1Id = "user1Id";
            var user2Id = "user2Id";

            var match = _service.GetCurrentMatch();
            _service.GetRandomNumberForUser(user1Id);
            _service.GetRandomNumberForUser(user2Id);
            _service.Submit(user1Id, "name1");
            _service.Submit(user2Id, "name2");
            match.ExpireTime = DateTime.Now;
            
            match = _service.GetCurrentMatch();
            _service.GetRandomNumberForUser(user1Id);
            _service.GetRandomNumberForUser(user2Id);
            _service.Submit(user1Id, "name1");
            _service.Submit(user2Id, "name2");
            match.ExpireTime = DateTime.Now;

            var winners = _service.GetLastWinners();

            winners.Should().BeOfType<List<Game>>();
            winners.Count.Should().Be(2);
        }

        [Test]
        public void TestThatWinnerIsWithGreatestNumber()
        {
            var user1Id = "user1Id";
            var user2Id = "user2Id";

            var match = _service.GetCurrentMatch();
            var number1 = _service.GetRandomNumberForUser(user1Id);
            var number2 = _service.GetRandomNumberForUser(user2Id);
            _service.Submit(user1Id, "name1");
            _service.Submit(user2Id, "name2");
            match.ExpireTime = DateTime.Now;

            var winners = _service.GetLastWinners();
            var winningNumber = Math.Max(number1, number2);
            
            winners.FirstOrDefault(x => x.MatchId == match.Id)?.Number.Should().Be(winningNumber);
        }

        [Test]
        public void SubmitShouldRecordUserNameAndUserId()
        {
            var userId = "id";
            var userName = "Borche Ivanov";

            _service.GetCurrentMatch();
            _service.GetRandomNumberForUser(userId);
            var submittedGame = _service.Submit(userId, userName);

            submittedGame.UserId.Should().Be(userId);
            submittedGame.UserName.Should().Be(userName);
        }

        [Test]
        public void SubmitShouldThrowIfTheUserTriesToSubmitAgain()
        {
            var userId = "newId";
            var userName = "name";

            _service.GetCurrentMatch();
            _service.GetRandomNumberForUser(userId);
            _service.Submit(userId, userName);

            try
            {
                _service.GetRandomNumberForUser(userId);;
               _service.Submit(userId, userName);
               Assert.Fail(); 
            }
            catch(Exception e)
            {
                e.Message.Should().Be("user allready submitted");
            }
        }
    }
}