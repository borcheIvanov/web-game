using System.Threading.Tasks;
using web_game.Models;

namespace web_game.Repositories
{
    public class Repository : IRepository
    {
        public Game Add(Game entity)
        {
            // _context.Attach(entity).State = EntityState.Added;
            // await _context.SaveChangesAsync();
            return entity;
        }
    }
}