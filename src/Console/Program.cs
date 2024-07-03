using Azure.Identity;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Messaging.ServiceBus;
using CommandLine;
using RobinThoene.DefaultAzureAuthDebug.Console;
using Azure.Core;

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
try
{
    var tokenResponse = credential.GetToken(new TokenRequestContext(new string[] { "https://graph.microsoft.com" }));
    if (!string.IsNullOrEmpty(tokenResponse.Token))
    {
        logger.LogInformation("Token was retrieved successful.");
        if (arguments.LogToken)
        {
            logger.LogInformation("Your retrieved token: {0}", tokenResponse.Token);
        }
    }
    else
    {
        logger.LogError("Could not retrieve a token using default azure credential.");
    }
}
catch (Exception ex)
{
    if (arguments.Verbose)
    {
        logger.LogError("Token retrieval failed with exception: {0}", ex.Message);
    }
    else
    {
        logger.LogError("Token retrieval failed with exception. Use the 'verbose' argument to see the exception message.");
    }
}
if (!string.IsNullOrEmpty(arguments.StorageAccountName))
{
    logger.LogInformation("Trying to access the storage account {0} ", arguments.StorageAccountName);
    try
    {
        // Check if access to the blob storage account is possible.
        var blobServiceClient = new BlobServiceClient(new Uri($"https://{arguments.StorageAccountName}.blob.core.windows.net"), credential);
        var blobServiceResponse = blobServiceClient.GetProperties().GetRawResponse();
        if (blobServiceResponse.Status is 200)
        {
            logger.LogInformation("Access to the storage account was successful.");
        }
        else
        {
            logger.LogError("Access to the storage account failed with status code {0}", blobServiceResponse.Status);
        }
    }
    catch (Exception ex)
    {
        if (arguments.Verbose)
        {
            logger.LogError("Access to the storage account failed with exception: {0}", ex.Message);
        }
        else
        {
            logger.LogError("Access to the storage account failed with exception. Use the 'verbose' argument to see the exception message.");
        }
    }
}
if (!string.IsNullOrEmpty(arguments.ServiceBusNamespace) && !string.IsNullOrEmpty(arguments.ServiceBusQueueName))
{
    logger.LogInformation("Trying to access service bus queue {0} ", arguments.ServiceBusQueueName);
    try
    {
        // Check if access to the service bus is possible.
        var serviceBusClient = new ServiceBusClient($"{arguments.ServiceBusNamespace}.servicebus.windows.net", credential);
        var serviceBusQueueReceiver = serviceBusClient.CreateReceiver(arguments.ServiceBusQueueName);
        logger.LogInformation("Waiting for a service bus message to receive ...");
        var message = await serviceBusQueueReceiver.ReceiveMessageAsync();
        if (message is not null)
        {
            logger.LogInformation("Received message from the service bus with content: {0}", message.Body.ToString());
        }
        else
        {
            logger.LogInformation("No message available on the service bus.");
        }
    }
    catch (Exception ex)
    {
        if (arguments.Verbose)
        {
            logger.LogError("Access to the service bus queue failed with exception: {0}", ex.Message);
        }
        else
        {
            logger.LogError("Access to the service bus queue failed with exception. Use the 'verbose' argument to see the exception message.");
        }
    }
}

