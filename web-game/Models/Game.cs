using System;
using System.ComponentModel.DataAnnotations;

namespace web_game.Models
{
    public class Game
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MatchId { get; set; }
        public Guid UserId { get; set; } = Guid.Empty;
        public int Number { get; set; }
        public Match Match { get; set; } = null!;
    }
}