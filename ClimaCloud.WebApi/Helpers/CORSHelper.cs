namespace ClimaCloud.WebApi.Helpers
{
    internal static class CORSHelper
    {
        public static List<string> _endPointCache = new List<string>();

        public static List<string> CreateCorsAllowedHosts(int start, int count)
        {
            if (_endPointCache.Count() > 0) return _endPointCache;

            var endPoints = new List<string> {
                "https://localhost:7167",
                "https://localhost:7272",
                "https://localhost:7199",
                "https://www.climacloud.co",
                "https://www.climacloud.co"
            };

            _endPointCache = endPoints;
            return endPoints;
        }
    }
}
