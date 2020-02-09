using System;
using System.ComponentModel.DataAnnotations;

namespace web_game.Models
{
    public class Game
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MatchId { get; set; }
        public string UserId { get; set; } = null!;
        public int Number { get; set; }
    }
}