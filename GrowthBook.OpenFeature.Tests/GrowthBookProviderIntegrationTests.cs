using System.Collections.Generic;
using System.Threading.Tasks;
using OpenFeature;
using OpenFeature.Model;
using OpenFeature.Constant;
using Xunit;

namespace GrowthBook.OpenFeature.Tests;

/// <summary>
/// Integration tests for the GrowthBook Provider
/// Note: These tests don't connect to an actual GrowthBook server, but use a local GrowthBook instance with pre-configured features
/// </summary>
public class GrowthBookProviderIntegrationTests
{
    [Fact]
    public async Task IntegrationTest_WithLocalGrowthBookInstance_EvaluatesBooleanFlag()
    {
        // Arrange - Create a GrowthBook instance with pre-configured features
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Add a test feature
        var features = new Dictionary<string, global::GrowthBook.Feature>
        {
            { 
                "test-bool-flag", 
                new global::GrowthBook.Feature 
                { 
                    DefaultValue = true
                }
            }
        };
        growthBook.Features = features;
        
        // Create and register the provider
        var provider = new GrowthBookProvider(growthBook);
        await global::OpenFeature.Api.Instance.SetProviderAsync(provider);
        
        // Act - Get a client and evaluate a flag
        var client = global::OpenFeature.Api.Instance.GetClient();
        var result = await client.GetBooleanValueAsync("test-bool-flag", false);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IntegrationTest_WithLocalGrowthBookInstance_EvaluatesStringFlag()
    {
        // Arrange - Create a GrowthBook instance with pre-configured features
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Add a test feature
        var features = new Dictionary<string, global::GrowthBook.Feature>
        {
            { 
                "test-string-flag", 
                new global::GrowthBook.Feature 
                { 
                    DefaultValue = "feature-value"
                }
            }
        };
        growthBook.Features = features;
        
        // Create and register the provider
        var provider = new GrowthBookProvider(growthBook);
        await global::OpenFeature.Api.Instance.SetProviderAsync(provider);
        
        // Act - Get a client and evaluate a flag
        var client = global::OpenFeature.Api.Instance.GetClient();
        var result = await client.GetStringValueAsync("test-string-flag", "default");
        
        // Assert
        Assert.Equal("feature-value", result);
    }
    
    [Fact]
    public async Task IntegrationTest_WithLocalGrowthBookInstance_EvaluatesNumberFlag()
    {
        // Arrange - Create a GrowthBook instance with pre-configured features
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Add a test feature
        var features = new Dictionary<string, global::GrowthBook.Feature>
        {
            { 
                "test-number-flag", 
                new global::GrowthBook.Feature 
                { 
                    DefaultValue = 42
                }
            }
        };
        growthBook.Features = features;
        
        // Create and register the provider
        var provider = new GrowthBookProvider(growthBook);
        await global::OpenFeature.Api.Instance.SetProviderAsync(provider);
        
        // Act - Get a client and evaluate a flag
        var client = global::OpenFeature.Api.Instance.GetClient();
        var result = await client.GetIntegerValueAsync("test-number-flag", 0);
        
        // Assert
        Assert.Equal(42, result);
    }
    
    [Fact]
    public async Task IntegrationTest_WithContextTargeting_PassesContextToGrowthBook()
    {
        // Arrange - Create a GrowthBook instance with pre-configured features
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Add a test feature
        var features = new Dictionary<string, global::GrowthBook.Feature>
        {
            { 
                "test-targeting-flag", 
                new global::GrowthBook.Feature 
                { 
                    DefaultValue = false
                }
            }
        };
        growthBook.Features = features;
        
        // Create and register the provider
        var provider = new GrowthBookProvider(growthBook);
        await global::OpenFeature.Api.Instance.SetProviderAsync(provider);
        
        // Create an evaluation context with targeting key
        var targetingKey = "user-123";
        var evalContextBuilder = EvaluationContext.Builder();
        evalContextBuilder.Set("targetingKey", targetingKey);
        var evalContext = evalContextBuilder.Build();
        
        // Act - Get a client and evaluate a flag with the context
        var client = global::OpenFeature.Api.Instance.GetClient("test-client");
        client.SetContext(evalContext);
        
        await client.GetBooleanValueAsync("test-targeting-flag", false);
        
        // Assert - Check that the GrowthBook instance has the expected attributes
        // In a real integration test, you might verify this by checking that the correct rule was evaluated
        Assert.NotNull(growthBook.Attributes);
        Assert.Equal("user-123", growthBook.Attributes["id"]?.ToString());
    }
    
    [Fact]
    public async Task IntegrationTest_WithDetailedFlagEvaluation_ProvidesMeaningfulMetadata()
    {
        // Arrange - Create a GrowthBook instance with pre-configured features
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Add a test feature
        var features = new Dictionary<string, global::GrowthBook.Feature>
        {
            { 
                "detailed-flag", 
                new global::GrowthBook.Feature 
                { 
                    DefaultValue = true
                }
            }
        };
        growthBook.Features = features;
        
        // Create and register the provider
        var provider = new GrowthBookProvider(growthBook);
        await global::OpenFeature.Api.Instance.SetProviderAsync(provider);
        
        // Act - Get a client and evaluate a flag
        var client = global::OpenFeature.Api.Instance.GetClient();
        var evalDetails = await client.GetBooleanDetailsAsync("detailed-flag", false);
        
        // Assert
        Assert.True(evalDetails.Value);
        Assert.Equal("TARGETING_MATCH", evalDetails.Reason);
        Assert.Equal(ErrorType.None, evalDetails.ErrorType);
    }
    
    /// <summary>
    /// Helper class for creating evaluation contexts for testing
    /// </summary>
    private static class TestContextHelper
    {
        public static EvaluationContext CreateContext(string targetingKey)
        {
            var builder = EvaluationContext.Builder();
            builder.Set("targetingKey", targetingKey);
            return builder.Build();
        }
    }
} 