# GrowthBook OpenFeature Provider for .NET

[![.NET CI](https://github.com/growthbook/growthbook-openfeature-provider-dot-net/actions/workflows/ci.yml/badge.svg)](https://github.com/growthbook/growthbook-openfeature-provider-dot-net/actions/workflows/ci.yml)

This is the official GrowthBook provider for the [OpenFeature .NET SDK](https://github.com/open-feature/dotnet-sdk).

## Requirements

- .NET 8+
- .NET Framework 4.6.2+

## Installation

```bash
dotnet add package GrowthBook.OpenFeature
```

## Usage

```csharp
using GrowthBook.OpenFeature;
using OpenFeature;
using OpenFeature.Model;

// Initialize the GrowthBook provider
var provider = new GrowthBookProvider(
    apiKey: "YOUR_API_KEY",
    clientKey: "YOUR_CLIENT_KEY",
    hostUrl: "https://cdn.growthbook.io"
);

// Register the provider with OpenFeature
Api.Instance.SetProvider(provider);

// Create an evaluation context
var context = new EvaluationContext(
    targetingKey: "user-123",
    new Dictionary<string, Value>
    {
        { "email", new Value("user@example.com") },
        { "country", new Value("USA") }
    }
);

// Get the client
var client = Api.Instance.GetClient();

// Evaluate a feature flag
bool isEnabled = await client.GetBooleanValue("feature-flag-key", false, context);

// Use the feature flag
if (isEnabled)
{
    // Feature is enabled
}
else
{
    // Feature is disabled
}
```

## Advanced Usage

You can also use an existing GrowthBook instance:

```csharp
// Create GrowthBook context
var gbContext = new GrowthBook.Context
{
    ApiHost = "https://cdn.growthbook.io",
    ClientKey = "YOUR_CLIENT_KEY"
};

// Create a GrowthBook instance
var growthBook = new GrowthBook.GrowthBook(gbContext);

// Initialize the GrowthBook provider with the existing instance
var provider = new GrowthBookProvider(growthBook);

// Register the provider with OpenFeature
Api.Instance.SetProvider(provider);
```

## License

MIT
