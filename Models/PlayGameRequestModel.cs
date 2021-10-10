using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hangman.Models
{
    public class PlayGameRequestModel
    {

        /// <summary>
        /// Enter your guess
        /// </summary>
        [Required]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Please enter one character")]
        [JsonProperty("guessd_character")]
        public string GuessedCharacter { get; set; }
    }
}
