using Azure.Identity;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Messaging.ServiceBus;
using CommandLine;
using RobinThoene.DefaultAzureAuthDebug.Console;

// Read the cli arguments.
var arguments = Parser.Default.ParseArguments<Options>(args).Value;
// Configure a logger.
using var factory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(arguments?.Verbose ?? false ? LogLevel.Debug : LogLevel.Information);
    builder.AddConsole();
});
var logger = factory.CreateLogger("Program");
// Setup default azure credentials for authentication.
var defaultAzureCredOptions = new DefaultAzureCredentialOptions
{
};
var credential = new DefaultAzureCredential(defaultAzureCredOptions);
logger.LogInformation("Getting credentials ...");
if (!string.IsNullOrEmpty(arguments.StorageAccountName))
{
    logger.LogInformation("Trying to access the storage account {0} ", arguments.StorageAccountName);
    // Check if access to the blob storage account is possible.
    var blobServiceClient = new BlobServiceClient(new Uri($"https://{arguments.StorageAccountName}.blob.core.windows.net"), credential);
    logger.LogInformation(blobServiceClient.GetProperties().ToString());
}
if (!string.IsNullOrEmpty(arguments.ServiceBusNamespace) && !string.IsNullOrEmpty(arguments.ServiceBusQueueName))
{
    // Check if access to the service bus is possible.
    var serviceBusClient = new ServiceBusClient($"{arguments.ServiceBusNamespace}.servicebus.windows.net", credential);
    var serviceBusQueueReceiver = serviceBusClient.CreateReceiver(arguments.ServiceBusQueueName);
    logger.LogInformation("Waiting for a service bus message to receive ...");
    var message = await serviceBusQueueReceiver.ReceiveMessageAsync();
    if (message is not null)
    {
        logger.LogInformation("Received message from the service bus with content: {0}", message.Body.ToString());
        await serviceBusQueueReceiver.CompleteMessageAsync(message);
    }
    else
    {
        logger.LogInformation("No message available on the service bus.");
    }
}

