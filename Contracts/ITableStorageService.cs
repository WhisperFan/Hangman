using Hangman.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangman.Contracts
{
    public interface ITableStorageService 
    {
        Task<GameTableEntity> RetrieveAsync(string id);
        Task<GameTableEntity> InsertOrMergeAsync(GameTableEntity entity);
        Task<GameTableEntity> DeleteAsync(GameTableEntity entity);
        Task<IEnumerable<GameTableEntity>> RetrieveAllFinishedAsync();

        Task<IEnumerable<GameTableEntity>> RetrieveAllActiveAsync();
    }
}
