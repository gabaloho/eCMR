# modelCmr

modelCmr is a .NET MAUI application prototype for viewing shipment records and
creating electronic CMR transport documents. It retrieves shipment data from an
existing API, displays the details in a CMR-style preview, and can map the
selected shipment to a transport-document request for the eWastra API.

## Current functionality

- Retrieves CMR-related shipment records and nested shipment data from an API.
- Displays shipment, consignor, consignee, carrier, goods, pallet, and ADR
  information.
- Builds and submits a transport-document payload to eWastra.
- Supports OAuth 2.0 client-credentials authentication for the eWastra test API.

This is an early-stage prototype. It does not currently provide local sample
data or an offline mode, and its API configuration and credential handling need
additional work before production use.

## Technology

- C# and .NET 9
- .NET MAUI with XAML
- `HttpClient` and `System.Text.Json`

The project targets Android, iOS, and Mac Catalyst. It also includes a
Windows-specific target when built on Windows.

## Requirements

- .NET 9 SDK
- The .NET MAUI workload and the platform-specific tools required by the target
  (for example, Android SDK/emulator tools for Android)
- Network access to restore NuGet packages
- Access to the shipment API for loading live data

## Build and run

Restore the project workloads and packages:

```sh
dotnet workload restore modelCmr.csproj
dotnet restore modelCmr.csproj
```

Build for a platform with its required tooling installed:

```sh
dotnet build modelCmr.csproj --framework net9.0-android
```

Run or deploy the app using the appropriate .NET MAUI tooling for your target
device or emulator. The available target frameworks and installed workloads
vary by operating system.

## API configuration and limitations

The shipment API base address is currently hard-coded in
`Services/CustomHttpMessageHandler.cs` and points to a private-network address.
Change it to an API endpoint reachable from the device before expecting the
shipment list to load.

The eWastra API defaults in `Views/CmrPreviewPage.xaml.cs` point to the
eWastra test environment. Sending a document requires network access and valid
API credentials, which the app currently prompts for during submission. The
app has not been documented or tested here as a production-ready integration.

For development builds, the custom HTTP handler accepts any server certificate.
This disables certificate validation in Debug builds and must not be used as a
production security configuration.

## License

This project is available under the MIT License. See [LICENSE](LICENSE).
