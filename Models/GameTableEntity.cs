using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangman.Models
{
    public class GameTableEntity : TableEntity
    {
        public GameTableEntity()
        {
        }
        public GameTableEntity(string id, string selectedWord, string result) : base("DefaultPlayer", id)
        {
            Player = "DefaultPlayer";
            Id = id;
            SelectedWord = selectedWord;
            Result = result;
            NumberOfTries = selectedWord.Length;
            var positionArray = new string[] { };
            CorrectPositions = string.Join(",", positionArray);
        }
        /// <summary>
        /// Name of the player (since there is no authentication, there will be only one player, the default player)
        /// </summary>
        public string Player { get; set; }

        /// <summary>
        /// The GUID id of the game
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The selected word
        /// </summary>
        public string SelectedWord { get; set; }

        /// <summary>
        /// Number of tries in total
        /// </summary>
        public int NumberOfTries { get; set; }

        /// <summary>
        /// The result of the game
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Correctly guessed positions
        /// </summary>
        public string CorrectPositions { get; set; }

    }
}
