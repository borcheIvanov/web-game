using System;
using System.ComponentModel.DataAnnotations;

namespace web_game.Models
{
    public class Game
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Match If foreign key
        /// </summary>
        public Guid MatchId { get; set; }
        /// <summary>
        /// Match navigational property
        /// </summary>
        public Match Match { get; set; } = null!;
        /// <summary>
        /// The id of the user that submitted to a specific game
        /// </summary>
        public Guid UserId { get; set; } = Guid.Empty;
        /// <summary>
        /// The number submitted by the User 
        /// </summary>
        public int Number { get; set; }
        
    }
}