using System;
using System.ComponentModel.DataAnnotations;

namespace web_game.Models
{
    public class Match
    {
        [Key]
        public Guid Id = Guid.NewGuid();
        public DateTime ExpireTime { get; set; }
        
        public TimeSpan RemainingTime => ExpireTime - DateTime.Now;
    }
}