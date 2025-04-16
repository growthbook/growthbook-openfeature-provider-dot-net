# GrowthBook OpenFeature Provider for .NET

[![.NET CI](https://github.com/growthbook/growthbook-openfeature-provider-dot-net/actions/workflows/ci.yml/badge.svg)](https://github.com/growthbook/growthbook-openfeature-provider-dot-net/actions/workflows/ci.yml)

This is the official [GrowthBook](https://www.growthbook.io/) provider for the [OpenFeature .NET SDK](https://github.com/open-feature/dotnet-sdk). It allows you to leverage GrowthBook as your feature flag system through the standardized OpenFeature API.

## Quick Start

1. Install the package
2. Create a GrowthBook provider
3. Register with OpenFeature
4. Start evaluating feature flags

## Requirements

- .NET 8+
- .NET Framework 4.6.2+

## Installation

```bash
dotnet add package GrowthBook.OpenFeature
```

## Basic Usage

```csharp
using GrowthBook.OpenFeature;
using OpenFeature;
using OpenFeature.Model;

// Initialize the GrowthBook provider
var provider = new GrowthBookProvider(
    clientKey: "sdk-abc123",
    apiHostUrl: "https://cdn.growthbook.io"
);

// Register the provider with OpenFeature
Api.Instance.SetProvider(provider);

// Create an evaluation context with targeting information
var context = new EvaluationContext(
    targetingKey: "user-123",
    new Dictionary<string, Value>
    {
        { "email", new Value("user@example.com") },
        { "country", new Value("USA") },
        { "premium", new Value(true) }
    }
);

// Get the client
var client = Api.Instance.GetClient();

// Evaluate a feature flag
bool isEnabled = await client.GetBooleanValue("new-dashboard", false, context);

// Use the feature flag in your application logic
if (isEnabled)
{
    // Show the new dashboard
}
else
{
    // Show the old dashboard
}
```

## Available Flag Types

This provider supports all OpenFeature evaluation types:

```csharp
// Boolean flags
bool enabled = await client.GetBooleanValue("feature-enabled", false, context);

// String flags
string variant = await client.GetStringValue("button-color", "blue", context);

// Integer flags
int limit = await client.GetIntegerValue("api-request-limit", 100, context);

// Double flags
double price = await client.GetDoubleValue("subscription-price", 9.99, context);

// Object flags
Value config = await client.GetObjectValue("feature-config", new Value(new Dictionary<string, Value>()), context);
```

## Advanced Usage

### Using an Existing GrowthBook Instance

If you already have a GrowthBook instance configured:

```csharp
// Create GrowthBook context
var gbContext = new GrowthBook.Context
{
    ApiHost = "https://cdn.growthbook.io",
    ClientKey = "sdk-abc123",
    Enabled = true,
    // Add additional configuration as needed
};

// Create and configure a GrowthBook instance
var growthBook = new GrowthBook.GrowthBook(gbContext);

// Initialize the GrowthBook provider with the existing instance
var provider = new GrowthBookProvider(growthBook);

// Register the provider with OpenFeature
Api.Instance.SetProvider(provider);
```

### Handling Context Changes

You can update the evaluation context at any time:

```csharp
// Create new context when user logs in
var userContext = new EvaluationContext(
    targetingKey: "user-456", 
    new Dictionary<string, Value>
    {
        { "email", new Value("new-user@example.com") },
        { "subscriptionTier", new Value("premium") }
    }
);

// Update the client with the new context
client.SetEvaluationContext(userContext);

// All subsequent evaluations will use the new context
var result = await client.GetBooleanValue("premium-feature", false);
```

## Project Structure

The solution consists of:

- **GrowthBook.OpenFeature**: The main library implementing the OpenFeature provider
- **GrowthBook.OpenFeature.Tests**: Unit tests for the provider

## Troubleshooting

### Common Issues

1. **Feature flag not found**: Ensure your `clientKey` is correct and that the flag exists in your GrowthBook project
2. **Incorrect targeting**: Check that your evaluation context contains the expected targeting attributes
3. **No connection to GrowthBook**: Verify your `apiHostUrl` is correct and network connectivity is available

### Debugging

Enable verbose logging to see detailed evaluation information:

```csharp
OpenFeature.Api.Instance.SetLoggerFactory(new ConsoleLoggerFactory(LogLevel.Debug));
```

## Contributing

Contributions are welcome! See [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## License

MIT
