using System;
using FakeItEasy;
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
        private readonly IMatchesService _service;

        public ServiceTests()
        {
            _gameRepository = A.Fake<IRepository>();
            _service = new MatchesService(_gameRepository);
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
            var result = _service.GetRandomNumberForUser(Guid.NewGuid());

            result.Should().BeGreaterThan(0);
            result.Should().BeLessOrEqualTo(100);
        }

        [Test]
        public void GetRandom_ShouldReturnSameNumberForSameMatch()
        {
            var userId = Guid.NewGuid();
            var result = _service.GetRandomNumberForUser(userId);
            var result2 = _service.GetRandomNumberForUser(userId);

            result.Should().Be(result2);
        }

        [Test]
        public void GetRandom_ShouldReturnDifferentNumberForSameMatchForDifferentUser()
        {
            var userId = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            var result = _service.GetRandomNumberForUser(userId);
            var result2 = _service.GetRandomNumberForUser(userId2);

            result.Should().NotBe(result2);
        }

        [Test]
        public void GetRandom_ShouldReturnDifferentNumberIfMatchExpired()
        {
            var userId = Guid.NewGuid();
            var match = _service.GetCurrentMatch();

            var result = _service.GetRandomNumberForUser(userId);
            match.ExpireTime = DateTime.Now;
            var result2 = _service.GetRandomNumberForUser(userId);

            result.Should().NotBe(result2);
        }


        [Test]
        public void UserCanSubmitNumberToCurrentGame()
        {
            var userId = Guid.NewGuid();
            var match = _service.GetCurrentMatch();
            var rand = _service.GetRandomNumberForUser(userId);
            var game = new Game {UserId = userId, Number = rand, MatchId = match.Id, Match = match};
            A.CallTo(() => _gameRepository.Add(A<Game>.Ignored)).Returns(game);

            _service.Submit(userId);
            A.CallTo(() => _gameRepository.Add(A<Game>.Ignored)).MustHaveHappened();

            game.Match.Id.Should().Be(match.Id);
            game.UserId.Should().Be(userId);
            game.Number.Should().Be(rand);
        }

        [Test]
        public void ExceptionShouldBeThrown_ThereIsNoGeneratedNumber()
        {
            var userId = Guid.NewGuid();
            var match = _service.GetCurrentMatch();

            var exceptionHappened = false;

            try
            {
                _service.Submit(userId);
            }
            catch (Exception e)
            {
                exceptionHappened = true;
                e.Message.Should().Be($"A number was not generated for userId {userId}");
            }

            exceptionHappened.Should().Be(true);
            A.CallTo(() => _gameRepository.Add(A<Game>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void ExceptionShouldBeThrown_ThereIsNoNumberForTheCurrentMatch()
        {
            var userId = Guid.NewGuid();
            var match = _service.GetCurrentMatch();
            var rand = _service.GetRandomNumberForUser(userId);

            match.ExpireTime = DateTime.Now;
            var exceptionHappened = false;

            try
            {
                _service.Submit(userId);
            }
            catch (Exception e)
            {
                exceptionHappened = true;
                e.Message.Should().Be("Match Expired");
            }

            exceptionHappened.Should().Be(true);
            A.CallTo(() => _gameRepository.Add(A<Game>.Ignored)).MustNotHaveHappened();
        }
    }
}