using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text;

namespace ClimaCloud.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            if (!builder.HostEnvironment.IsDevelopment())
            {
                // Add support for reading the configuration
                string environment = builder.HostEnvironment.Environment.ToLowerInvariant();
                Console.WriteLine(environment);

                var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
                try
                {
                    var response = await httpClient.GetAsync($"appsettings.{environment}.json");

                    Console.WriteLine("StatusCode:" + response.StatusCode);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonContent = await response.Content.ReadAsStringAsync();
                        builder.Configuration.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonContent)));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading configuration: " + ex.ToString());
                }
            }

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}