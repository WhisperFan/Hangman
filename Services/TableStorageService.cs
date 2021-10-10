using Hangman.Contracts;
using Hangman.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangman.Services
{
    public class TableStorageService : ITableStorageService
    {
        private const string TableName = "Games";
        private const string PartitionKey = "DefaultPlayer";
        private readonly IConfiguration _configuration;
        public TableStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GameTableEntity> RetrieveAsync(string id)
        {
            var retrieveOperation = TableOperation.Retrieve<GameTableEntity>(PartitionKey, id);
            return await ExecuteTableOperation(retrieveOperation) as GameTableEntity;
        }
        public async Task<IEnumerable<GameTableEntity>> RetrieveAllActiveAsync()
        {
            var table = await GetCloudTable();
            var results = table.ExecuteQuery(new TableQuery<GameTableEntity>()).ToList().Where(p => p.Result == "Started");
           
            return results;
        }

        public async Task<IEnumerable<GameTableEntity>> RetrieveAllFinishedAsync()
        {
            var table = await GetCloudTable();
            var results = table.ExecuteQuery(new TableQuery<GameTableEntity>()).ToList().Where(p => p.Result == "Succeed" || p.Result == "Failed");
            return results;
        }
        public async Task<GameTableEntity> InsertOrMergeAsync(GameTableEntity entity)
        {
            var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            return await ExecuteTableOperation(insertOrMergeOperation) as GameTableEntity;
        }
        public async Task<GameTableEntity> DeleteAsync(GameTableEntity entity)
        {
            var deleteOperation = TableOperation.Delete(entity);
            return await ExecuteTableOperation(deleteOperation) as GameTableEntity;
        }
        private async Task<object> ExecuteTableOperation(TableOperation tableOperation)
        {
            var table = await GetCloudTable();
            var tableResult = await table.ExecuteAsync(tableOperation);
            return tableResult.Result;
        }
        private async Task<CloudTable> GetCloudTable()
        {
            var storageAccount = CloudStorageAccount.Parse(_configuration["StorageConnectionString"]);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

    }
}
