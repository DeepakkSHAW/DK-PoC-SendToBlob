using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

var storageConn = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

if (string.IsNullOrEmpty(storageConn) )
{ 
    throw new InvalidOperationException(
        "Environment variables missing configuration. Please specify them to continue...");
}

builder.Services.AddSingleton(factory => new BlobServiceClient(storageConn));

builder.Build().Run();
