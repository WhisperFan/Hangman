using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangman.Models
{
    public class StartGameReturnResult
    {
        /// <summary>
        /// Legth of the selected word
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Number of tries left
        /// </summary>
        public int NumberOfTriesLeft { get; set; }

        /// <summary>
        /// Id of the game
        /// </summary>
        public string Id { get; set; }
    }
}
