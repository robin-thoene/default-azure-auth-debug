# default-azure-auth-debug (daad)

## Summary

A simple dotnet command line tool to debug the usage of `new DefaultAzureCredential` on your local machine.

Using that tool you can test if the method `new DefaultAzureCredential` from the package `Azure.Identity` can
pickup your authentication details from you local machine and retrieve a token.

Furthermore, using the cli parameters, you can define resources in Azure that the retrieved credential will be used
to authenticate against. The output of the application will show you if your retrieved credential will have access
to those resources or not.
