using System.ComponentModel.DataAnnotations;

namespace web_game.Models
{
    public class SubmitViewModel
    {
        [Range(0, 100)] public int Number { get; set; }
    }
}