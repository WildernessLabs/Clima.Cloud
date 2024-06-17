using Azure;
using System.Text.Json.Serialization;

namespace ClimaCloud.Core.Common
{
    public abstract class EntityBase
    {
        public EntityBase() { }

        [JsonIgnore]
        public string PartitionKey { get; set; } = default!;
        [JsonIgnore]
        public string RowKey { get; set; } = default!;
        [JsonIgnore]
        public DateTimeOffset? Timestamp { get; set; } = default!;
        [JsonIgnore]
        public ETag ETag { get; set; } = default!;
    }
}