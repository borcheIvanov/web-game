using System;
using System.Collections.Generic;
using web_game.Data;
using web_game.Models;

namespace web_game.Repositories
{
    public class MatchRepository: Repository<Match>, IMatchRepository
    {
        public MatchRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}