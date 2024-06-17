namespace ClimaCloud.Core.Common
{
    public static class Keys
    {
        // Partition key for current device events by organization
        public static string DeviceEventPartitionKey(string orgId)
        {
            return $"EVENT_{orgId}";
        }

        // Row key for current device events by device and event
        public static string DeviceEventRowKey(string deviceId)
        {
            return $"DEVICE_{deviceId}";
        }
    }
}