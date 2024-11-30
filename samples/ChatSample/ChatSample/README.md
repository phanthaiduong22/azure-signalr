# Azure SignalR Service Chat Sample

This sample demonstrates how to use Azure SignalR Service with ASP.NET Core SignalR.

## Prerequisites

- .NET 8.0 SDK or later
- An Azure SignalR Service instance
- Git (for submodule dependencies)

## Setup

1. Initialize the required submodules:

```bash
git submodule update --init --recursive
```

2. Configure your Azure SignalR Service connection string:

   - Update `appsettings.json` by replacing the empty connection string:

    ```json
    {
      "Azure": {
        "SignalR": {
          "ConnectionString": "<your-connection-string>"
        }
      }
    }
    ```

## Running the Sample

1. Build and run the project:

```bash
dotnet build
dotnet run
```

You can also specify a custom port:

```bash
dotnet run --urls="http://localhost:8080"
```

2. Access the chat application through your web browser at `http://localhost:5050` (or your custom port if specified)
