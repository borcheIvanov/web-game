using System;
using System.ComponentModel.DataAnnotations;

namespace web_game.Models
{
    public class Match
    {
        /// <summary>
        ///     Unique identifier
        /// </summary>
        [Key] public Guid Id = Guid.NewGuid();

        /// <summary>
        ///     Expire time of the match after expiration of one another will be created
        /// </summary>
        public DateTime ExpireTime { get; set; }

        /// <summary>
        ///     Remaining of the expiration time
        /// </summary>
        public TimeSpan RemainingTime => ExpireTime - DateTime.Now;
    }
}