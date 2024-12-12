using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;

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
                var ct = new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = "application/json" };

                //* initiate Blob Container Blob Client*//
                var containerBlobsClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerBlobsClient.CreateIfNotExistsAsync();
                //await containerBlobsClient.UploadBlobAsync($"DK-Data_{DateTime.Now.ToString("dd-MMM-yy_hh-mm-ss-ffff")}.txt", GetStream(messageToWriteToFile));

                //* initiate Blob Client*//
                BlobClient blobClient = containerBlobsClient.GetBlobClient($"dk-blob_{DateTime.Now.ToString("ddMMyy_hhmmssfff")}.json");
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(messageToWriteToFile)))
                    await blobClient.UploadAsync(ms, ct); //* Set Blob Content type *//

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
