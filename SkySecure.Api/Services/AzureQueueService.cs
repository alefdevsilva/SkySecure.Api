using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;
using System.Text.Json;

namespace SkySecure.Api.Services
{
    public interface IAzureQueueService
    {
        Task EnqueueMessageAsync(string message);
    }

    public class AzureQueueService : IAzureQueueService
    {
        private readonly string _conn;
        private readonly string _queueName;
        private readonly ILogger<AzureQueueService> _logger;

        public AzureQueueService(IConfiguration config, ILogger<AzureQueueService> logger)
        {
            _conn = config["Azure:StorageConnection"] ?? throw new InvalidOperationException("Storage connection missing");
            _queueName = config["Azure:QueueName"] ?? "policy-tasks";
            _logger = logger;
        }

        public async Task EnqueueMessageAsync(string message)
        {
            try
            {
                var client = new QueueClient(_conn, _queueName);
                await client.CreateIfNotExistsAsync();
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
                await client.SendMessageAsync(base64);
                _logger.LogInformation("Queued message to {Queue}", _queueName);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
          
        }
    }
}
