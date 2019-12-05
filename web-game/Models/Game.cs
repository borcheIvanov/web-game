using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace web_game.Models
{
    public class Game
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MatchId { get; set; }
        public string UserId { get; set; }

        public int Number { get; set; }
    }
}