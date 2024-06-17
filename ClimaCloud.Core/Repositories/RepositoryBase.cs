using Azure;
using Azure.Data.Tables;

namespace ClimaCloud.Core.Repositories
{
    public abstract class RepositoryBase
    {
        protected TableClient TableClient;

        protected RepositoryBase(TableClient tableClient)
        {
            TableClient = tableClient;
        }

        protected async Task<T?> GetAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            return await GetAsync<T>(partitionKey, rowKey, null, default);
        }

        protected async Task<T?> GetAsync<T>(string partitionKey, string rowKey, CancellationToken cancellationToken = default) where T : class, ITableEntity, new()
        {
            return await GetAsync<T>(partitionKey, rowKey, null, cancellationToken);
        }

        protected async Task<T?> GetAsync<T>(string partitionKey, string rowKey, IEnumerable<string>? select = null, CancellationToken cancellationToken = default)
            where T : class, ITableEntity, new()
        {
            try
            {
                var result = await TableClient.GetEntityAsync<T>(partitionKey, rowKey, select, cancellationToken);
                return result.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        protected AsyncPageable<T>? GetListAsync<T>(string partitionKey, CancellationToken cancellationToken = default) where T : class, ITableEntity, new()
        {
            try
            {
                AsyncPageable<T> queryResults = TableClient.QueryAsync<T>(filter: $"PartitionKey eq '{partitionKey}'", cancellationToken: cancellationToken);
                return queryResults;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        protected AsyncPageable<T>? GetFullCloudListAsync<T>(string filter, CancellationToken cancellationToken = default) where T : class, ITableEntity, new()
        {
            try
            {
                AsyncPageable<T> queryResults = TableClient.QueryAsync<T>(filter, cancellationToken: cancellationToken);
                return queryResults;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        protected async Task<T?> AddAsync<T>(T item, CancellationToken cancellationToken = default) where T : class, ITableEntity, new()
        {
            var result = await TableClient.AddEntityAsync(item, cancellationToken);
            if (!result.IsError)
            {
                return await GetAsync<T>(item.PartitionKey, item.RowKey, cancellationToken);
            }
            else
            {
                return null;
            }
        }

        protected async Task<T?> UpsertAsync<T>(T item, CancellationToken cancellationToken = default) where T : class, ITableEntity, new()
        {
            var result = await TableClient.UpsertEntityAsync(item, TableUpdateMode.Replace, cancellationToken);
            if (!result.IsError)
            {
                return await GetAsync<T>(item.PartitionKey, item.RowKey, cancellationToken);
            }
            else
            {
                return null;
            }
        }

        protected async Task<bool> DeleteAsync<T>(string partitionKey, string rowKey, CancellationToken cancellationToken = default) where T : class, ITableEntity, new()
        {
            var response = await TableClient.DeleteEntityAsync(partitionKey, rowKey, cancellationToken: cancellationToken);
            return !response.IsError;
        }
    }
}
