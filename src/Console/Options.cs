using CommandLine;

namespace RobinThoene.DefaultAzureAuthDebug.Console;

public class Options
{
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option(
        's',
        "storage-account-name",
        Required = false,
        HelpText = "The name of the storage account that shall be accessed using the default credential."
    )]
    public string? StorageAccountName { get; set; }

    [Option(
        'b',
        "service-bus-namespace",
        Required = false,
        HelpText = "The service bus namespace that shall be accessed using the default credential."
    )]
    public string? ServiceBusNamespace { get; set; }

    [Option(
        'q',
        "service-bus-queue-name",
        Required = false,
        HelpText = "The name of the service bus queue that shall be accessed using the default credential."
    )]
    public string? ServiceBusQueueName { get; set; }
}
