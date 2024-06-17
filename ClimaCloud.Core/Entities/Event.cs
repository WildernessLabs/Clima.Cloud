using Azure.Data.Tables;
using ClimaCloud.Core.Common;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClimaCloud.Core.Entities
{
    public class Event : EntityBase, ITableEntity
    {
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [IgnoreDataMember]
        public Dictionary<string, object> Measurements
        {
            get
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(_measurements) ?? new();
            }
            set { _measurements = JsonSerializer.Serialize(value); }
        }

        [JsonIgnore]
        public string MeasurementsRaw
        {
            get { return _measurements; }
            set
            {
                _measurements = value;
            }
        }
        private string _measurements = "";

        public string DeviceId { get; set; }
        public string OrgId { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}