using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DK_PoC_SendToBlob
{
    public class FnSendToBlob
    {
        private readonly ILogger<FnSendToBlob> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        public FnSendToBlob(ILogger<FnSendToBlob> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }
        private static Stream GetStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        
        [Function(nameof(FnSendToBlob))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("==Save message to Blob==");
            try
            {
                string containerName = Environment.GetEnvironmentVariable("BLOB-ContainerName");
                string messageToWriteToFile = "{ \"name\": \"deepak\", \"id\": 3}";
                var contBlobs = _blobServiceClient.GetBlobContainerClient(containerName);
                await contBlobs.UploadBlobAsync($"DK-Data_{DateTime.Now.ToString("dd-MMM-yy_hh-mm-ss-ffff")}.txt", GetStream(messageToWriteToFile));

                var response = $"==Save message to Blob, successfully completed==";
                _logger.LogWarning(response);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {

                _logger.LogWarning($"Error Occurred: {ex.Message}");
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
