using Hangman.Contracts;
using Hangman.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hangman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangmanController : ControllerBase
    {
        private readonly ITableStorageService _storageService;
        private readonly ILogger<HangmanController> _logger;

        public HangmanController(ITableStorageService storageService, ILogger<HangmanController> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        /// <summary>
        /// Start a Hangman Game, pick a random word and create a mew record in the sorage table, note, you need to have specify a storage account connection to run this app
        /// </summary>
        /// <returns> The ID of the game</returns>
        /// <remarks>Start a Hangman Game, pick a random word and create a mew record in the sorage table.<br/> </remarks>
        [HttpPost("start")]
        public async Task<ActionResult> StartGame()
        {
            string selectedWord = ReadWords();//"test";
            var id = Guid.NewGuid().ToString();
            var newGame = new GameTableEntity(id, selectedWord, "Started");
            var result = await _storageService.InsertOrMergeAsync(newGame);
            if(!string.IsNullOrEmpty(result.Id))
                return Ok(new StartGameReturnResult {Length = selectedWord.Length, NumberOfTriesLeft = newGame.NumberOfTries, Id = newGame.Id });
            return BadRequest("Failed to start a new game");
        }

        ///// <summary>
        ///// Play one round,  note, you need to have specify a storage account connection to run this app
        ///// </summary>
        ///// <returns> All the corrected polistions, Total lenght, number of retires left and game result</returns>
        [HttpPut("play/{gameId}")]
        public async Task<ActionResult<PlayGameReturnModel>> PlayGame([FromRoute] Guid gameId, PlayGameRequestModel request)
        {
            var game = await _storageService.RetrieveAsync(gameId.ToString());
            if (!string.IsNullOrEmpty(game.Id))
            {
                if(game.Result == "Succeed")
                {
                    return Ok("This game is finished successfully, please choose another one.");
                }
                if (game.NumberOfTries > 0)
                {
                    var selectedWord = game.SelectedWord;
                    if (selectedWord.ToLower().Contains(request.GuessedCharacter.ToLower()))
                    {
                        var positions = AllIndexesOf(selectedWord, request.GuessedCharacter);
                        var correctPositions = new List<string> { };
                        if (game.CorrectPositions != null)
                        {
                             correctPositions = game.CorrectPositions.Split(",").ToList();
                        }


                        correctPositions.AddRange(positions);
                        correctPositions.RemoveAll(p => p == "");
                        game.CorrectPositions = string.Join(",",correctPositions.Distinct().ToArray());
                    }
                    else
                        game.NumberOfTries = game.NumberOfTries - 1;
                    var currentPolistions = game.CorrectPositions;
                    var numCorrectCharacters = currentPolistions.Split(",").ToList().Count();
                    if (numCorrectCharacters == game.SelectedWord.Length)
                    {
                        game.Result = "Succeed";
                        await _storageService.InsertOrMergeAsync(game);
                        return Ok(new PlayGameReturnModel { Positions = game.CorrectPositions, Length = selectedWord.Length, NumberOfTriesLeft = game.NumberOfTries, GameResult = game.Result });
                    }
                    if (game.NumberOfTries == 0)
                    {
                        game.Result = "Failed";
                        await _storageService.InsertOrMergeAsync(game);
                        return Ok($"Your Game is Failed. The word is {game.SelectedWord}");
                    }

                    await _storageService.InsertOrMergeAsync(game);
                    return Ok(new  { Positions= game.CorrectPositions, Length = selectedWord.Length, NumberOfTriesLeft = game.NumberOfTries, GameResult = game.Result });

                }

                return Ok($"Your Game is Failed. The word is {game.SelectedWord}");

            }
            return BadRequest("Please enter a correct Game ID.");

        }

        /// <summary>
        ///Get All games that are started but not finished
        /// </summary>
        /// <returns></returns>
        [HttpGet("activegames")]
        public async Task<ActionResult<IEnumerable<GameTableEntity>>> GetALlActiveGames()
        {
            var games = await _storageService.RetrieveAllActiveAsync();
            
            return Ok(games);
        }

        /// <summary>
        /// Get All games that are finished
        /// </summary>
        /// 
        [HttpGet("finishedgames")]
        public async Task<ActionResult<IEnumerable<GameTableEntity>>> GetALlFinishedGames()
        {
            var games = await _storageService.RetrieveAllFinishedAsync();
            return Ok(games);
        }

        ///// <summary>
        ///// A method to get all the positions of a char in a string
        ///// </summary>
        ///// <returns></returns>
        private List<string> AllIndexesOf(string str, string value)
        {
            List<string> indexes = new List<string>();
            for (int index = 0; ; index += 1)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add((index + 1).ToString());
            }
        }

        private string ReadWords()
        {
            string startupPath = System.IO.Directory.GetCurrentDirectory();
            var path =startupPath +  "\\Files\\1000-words.txt";
            var reader = new StreamReader(path);
            List<string> wordsList = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                wordsList.Add(line);
            }
            wordsList.RemoveAll(p=>p.Length <4);
            Random rnd = new Random();
            int num = rnd.Next(0, wordsList.Count()-1);
            return wordsList[num];
        }
    }
}
