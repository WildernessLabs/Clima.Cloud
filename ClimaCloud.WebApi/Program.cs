using Azure.Data.Tables;
using Azure.Identity;
using ClimaCloud.Core.Interfaces;
using ClimaCloud.Core.Repositories;
using ClimaCloud.Core.Services;
using ClimaCloud.WebApi.Helpers;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClimaCloud.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure JSON options
            builder.Services.Configure<JsonOptions>(options =>
            {
                // Optionally, to use custom or default PascalCase (no specific naming policy)
                options.SerializerOptions.PropertyNamingPolicy = null;
            });


            builder.Services.AddSingleton<TableServiceClient>(provider =>
            {
                var connectionString = builder.Configuration["ClimaCloudStorageConn"];
                var isConnectionUri = Uri.TryCreate(connectionString, UriKind.Absolute, out var connectionUri);

                var useDefaultAzureCredential = connectionString!.Contains("AccountKey=", StringComparison.OrdinalIgnoreCase)
                    && isConnectionUri;

                return useDefaultAzureCredential
                    ? new TableServiceClient(connectionUri, new DefaultAzureCredential())
                    : new TableServiceClient(connectionString);
            });

            builder.Services.AddSingleton<TableClient>(provider =>
            {
                var tableName = builder.Configuration["ClimaCloudTableName"];
                var tableServiceClient = provider.GetRequiredService<TableServiceClient>();

                var tableClient = tableServiceClient.GetTableClient(tableName);
                tableClient.CreateIfNotExists();

                return tableClient;
            });

            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IEventService, EventService>();

            // Add services to the container.
            var CrossOriginAllowPolicy = "_crossOriginAllowPolicy";
            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: CrossOriginAllowPolicy,
                                  policy =>
                                  {
                                      policy
                                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                                      .WithOrigins(CORSHelper.CreateCorsAllowedHosts(start: 100, count: 200).ToArray())
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });


            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower, false));
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ClimaCloud", Version = "v1" });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors(CrossOriginAllowPolicy);

            app.UseHttpsRedirection();

            app.MapControllers();

            app.UseDeveloperExceptionPage();

            app.Run();
        }
    }
}