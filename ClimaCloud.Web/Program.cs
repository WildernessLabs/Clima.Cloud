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

            if (!builder.HostEnvironment.BaseAddress.Contains("localhost"))
            {
                // Add support for reading the configuration
                var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
                try
                {
                    var environment = builder.HostEnvironment.BaseAddress.Contains("staging") ? "staging" : "production";
                    Console.WriteLine("Environment:" + environment);

                    var response = await httpClient.GetAsync($"appsettings.{environment}.json");

                    Console.WriteLine("StatusCode:" + response.StatusCode);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonContent = await response.Content.ReadAsStringAsync();
                        builder.Configuration.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonContent)));

                        Console.WriteLine("Server.BaseAddress:" + builder.Configuration["ServerApi:BaseAddress"]);
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