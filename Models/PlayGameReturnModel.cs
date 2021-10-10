using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangman.Models
{
    public class PlayGameReturnModel
    {
        /// <summary>
        /// The positions of the correct guess
        /// </summary>
        public string Positions { get; set; }

        /// <summary>
        /// Legth of the selected word
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Number of tries left
        /// </summary>
        public int NumberOfTriesLeft { get; set; }

        /// <summary>
        /// Game Result: Started, Success, Failed
        /// </summary>
        public string GameResult { get; set; }

    }
}
