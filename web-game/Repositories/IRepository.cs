using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using web_game.Models;

namespace web_game.Repositories
{
    public interface IRepository
    {
        Game Add(Game entity);
    }
}