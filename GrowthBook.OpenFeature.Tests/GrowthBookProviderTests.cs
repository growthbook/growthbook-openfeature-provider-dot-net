using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using OpenFeature;
using OpenFeature.Model;
using OpenFeature.Constant;
using Xunit;

namespace GrowthBook.OpenFeature.Tests;

public class GrowthBookProviderTests
{
    #region Constructor Tests
    
    [Fact]
    public void Constructor_WithApiCredentials_InitializesProvider()
    {
        // Act
        var provider = new GrowthBookProvider("client-key", "https://host.com", "decrypt-key");
        
        // Assert
        Assert.NotNull(provider);
    }
    
    [Fact]
    public void Constructor_WithGrowthBookInstance_InitializesProvider()
    {
        // Arrange
        var gbContext = new global::GrowthBook.Context();
        var gbSdk = new global::GrowthBook.GrowthBook(gbContext);
        
        // Act
        var provider = new GrowthBookProvider(gbSdk);
        
        // Assert
        Assert.NotNull(provider);
    }
    
    #endregion
    
    #region GetMetadata Tests
    
    [Fact]
    public void GetMetadata_ReturnsCorrectMetadata()
    {
        // Arrange
        var gbContext = new global::GrowthBook.Context();
        var gbSdk = new global::GrowthBook.GrowthBook(gbContext);
        var provider = new GrowthBookProvider(gbSdk);
        
        // Act
        var metadata = provider.GetMetadata();
        
        // Assert
        Assert.Equal("GrowthBook Feature Provider", metadata.Name);
    }
    
    #endregion

    #region ResolveBooleanValueAsync Tests
    
    [Fact]
    public async Task ResolveBooleanValueAsync_WithExistingFeature_ReturnsFeatureValue()
    {
        // Arrange: Create a real GrowthBook instance instead of a mock
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
        
        // Create provider with real GrowthBook
        var provider = new GrowthBookProvider(growthBook);
        
        // Act
        EvaluationContext? context = null;
        var cancellationToken = System.Threading.CancellationToken.None;
        var result = await provider.ResolveBooleanValueAsync("test-bool-flag", false, context, cancellationToken);
        
        // Assert
        Assert.True(result.Value);
        Assert.Equal("TARGETING_MATCH", result.Reason);
        Assert.Equal(ErrorType.None, result.ErrorType);
    }
    
    [Fact]
    public async Task ResolveBooleanValueAsync_WithNonExistingFeature_ReturnsDefaultValue()
    {
        // Arrange: Create a real GrowthBook instance instead of a mock
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Empty features dictionary
        growthBook.Features = new Dictionary<string, global::GrowthBook.Feature>();
        
        // Create provider with real GrowthBook
        var provider = new GrowthBookProvider(growthBook);
        
        // Act
        EvaluationContext? context = null;
        var cancellationToken = System.Threading.CancellationToken.None;
        var result = await provider.ResolveBooleanValueAsync("non-existing-flag", true, context, cancellationToken);
        
        // Assert
        Assert.True(result.Value);
        Assert.Equal("DEFAULT", result.Reason);
    }
    
    [Fact]
    public async Task ResolveBooleanValueAsync_WithContext_MapsContextCorrectly()
    {
        // Arrange: Create a real GrowthBook instance 
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Add a test feature
        var features = new Dictionary<string, global::GrowthBook.Feature>
        {
            { 
                "test-flag", 
                new global::GrowthBook.Feature 
                { 
                    DefaultValue = true
                }
            }
        };
        growthBook.Features = features;
        
        // Create provider with real GrowthBook
        var provider = new GrowthBookProvider(growthBook);
        
        // Create context with targeting key
        var builder = EvaluationContext.Builder();
        builder.Set("targetingKey", "user-123");
        var context = builder.Build();
        var cancellationToken = System.Threading.CancellationToken.None;
        
        // Act
        await provider.ResolveBooleanValueAsync("test-flag", false, context, cancellationToken);
        
        // Assert
        Assert.NotNull(growthBook.Attributes);
        Assert.Equal("user-123", growthBook.Attributes["id"]?.ToString());
    }
    
    #endregion
    
    #region ResolveStringValueAsync Tests
    
    [Fact]
    public async Task ResolveStringValueAsync_WithExistingFeature_ReturnsFeatureValue()
    {
        // Arrange: Create a real GrowthBook instance instead of a mock
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
        
        // Create provider with real GrowthBook
        var provider = new GrowthBookProvider(growthBook);
        
        // Act
        EvaluationContext? context = null;
        var cancellationToken = System.Threading.CancellationToken.None;
        var result = await provider.ResolveStringValueAsync("test-string-flag", "default", context, cancellationToken);
        
        // Assert
        Assert.Equal("feature-value", result.Value);
        Assert.Equal("TARGETING_MATCH", result.Reason);
    }
    
    [Fact]
    public async Task ResolveStringValueAsync_WithNonExistingFeature_ReturnsDefaultValue()
    {
        // Arrange: Create a real GrowthBook instance instead of a mock
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Empty features dictionary
        growthBook.Features = new Dictionary<string, global::GrowthBook.Feature>();
        
        // Create provider with real GrowthBook
        var provider = new GrowthBookProvider(growthBook);
        
        // Act
        EvaluationContext? context = null;
        var cancellationToken = System.Threading.CancellationToken.None;
        var result = await provider.ResolveStringValueAsync("non-existing-flag", "default-string", context, cancellationToken);
        
        // Assert
        Assert.Equal("default-string", result.Value);
        Assert.Equal("DEFAULT", result.Reason);
    }
    
    #endregion
    
    #region ResolveIntegerValueAsync Tests
    
    [Fact]
    public async Task ResolveIntegerValueAsync_WithExistingFeature_ReturnsFeatureValue()
    {
        // Arrange: Create a real GrowthBook instance instead of a mock
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Add a test feature
        var features = new Dictionary<string, global::GrowthBook.Feature>
        {
            { 
                "test-int-flag", 
                new global::GrowthBook.Feature 
                { 
                    DefaultValue = 42
                }
            }
        };
        growthBook.Features = features;
        
        // Create provider with real GrowthBook
        var provider = new GrowthBookProvider(growthBook);
        
        // Act
        EvaluationContext? context = null;
        var cancellationToken = System.Threading.CancellationToken.None;
        var result = await provider.ResolveIntegerValueAsync("test-int-flag", 0, context, cancellationToken);
        
        // Assert
        Assert.Equal(42, result.Value);
        Assert.Equal("TARGETING_MATCH", result.Reason);
    }
    
    #endregion
    
    #region ResolveDoubleValueAsync Tests
    
    [Fact]
    public async Task ResolveDoubleValueAsync_WithExistingFeature_ReturnsFeatureValue()
    {
        // Arrange: Create a real GrowthBook instance instead of a mock
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Add a test feature
        var features = new Dictionary<string, global::GrowthBook.Feature>
        {
            { 
                "test-double-flag", 
                new global::GrowthBook.Feature 
                { 
                    DefaultValue = 3.14
                }
            }
        };
        growthBook.Features = features;
        
        // Create provider with real GrowthBook
        var provider = new GrowthBookProvider(growthBook);
        
        // Act
        EvaluationContext? context = null;
        var cancellationToken = System.Threading.CancellationToken.None;
        var result = await provider.ResolveDoubleValueAsync("test-double-flag", 0.0, context, cancellationToken);
        
        // Assert
        Assert.Equal(3.14, result.Value);
        Assert.Equal("TARGETING_MATCH", result.Reason);
    }
    
    #endregion
    
    #region ResolveStructureValueAsync Tests
    
    [Fact]
    public async Task ResolveStructureValueAsync_WithExistingFeature_ReturnsDefaultValue()
    {
        // Arrange: Create a real GrowthBook instance instead of a mock
        var gbContext = new global::GrowthBook.Context();
        var growthBook = new global::GrowthBook.GrowthBook(gbContext);
        
        // Add a test feature with a structure value (represented as Dictionary)
        var features = new Dictionary<string, global::GrowthBook.Feature>
        {
            { 
                "test-structure-flag", 
                new global::GrowthBook.Feature() 
            }
        };
        growthBook.Features = features;
        
        // Create provider with real GrowthBook
        var provider = new GrowthBookProvider(growthBook);
        var defaultValue = new Value(new Structure(new Dictionary<string, Value>()));
        
        // Act
        EvaluationContext? context = null;
        var cancellationToken = System.Threading.CancellationToken.None;
        var result = await provider.ResolveStructureValueAsync("test-structure-flag", defaultValue, context, cancellationToken);
        
        // Assert
        Assert.Equal(defaultValue, result.Value);
        Assert.Equal("TARGETING_MATCH", result.Reason);
    }
    
    #endregion
    
    #region Helper Methods
    
    private Mock<global::GrowthBook.GrowthBook> SetupMockGrowthBook()
    {
        var mockGb = new Mock<global::GrowthBook.GrowthBook>();
        // Setup necessary default behaviors
        mockGb.SetupProperty(g => g.Attributes);
        return mockGb;
    }
    
    private EvaluationContext CreateTestContext()
    {
        var builder = EvaluationContext.Builder();
        builder.Set("targetingKey", "user-123");
        builder.Set("customAttr", "custom-value");
        return builder.Build();
    }
    
    #endregion
    
    #region Test Helper Methods

    // TestContextHelper is used instead of the previous MutableEvaluationContext class
    private static class TestContextHelper
    {
        public static EvaluationContext CreateContext(string targetingKey)
        {
            var builder = EvaluationContext.Builder();
            if (!string.IsNullOrEmpty(targetingKey))
            {
                builder.Set("targetingKey", targetingKey);
            }
            return builder.Build();
        }
        
        public static EvaluationContext CreateContext(string targetingKey, Dictionary<string, object> attributes)
        {
            var builder = EvaluationContext.Builder();
            if (!string.IsNullOrEmpty(targetingKey))
            {
                builder.Set("targetingKey", targetingKey);
            }
            
            if (attributes != null)
            {
                foreach (var attr in attributes)
                {
                    if (attr.Value != null)
                    {
                        if (attr.Value is string strValue)
                        {
                            builder.Set(attr.Key, strValue);
                        }
                        else if (attr.Value is bool boolValue)
                        {
                            builder.Set(attr.Key, boolValue);
                        }
                        else if (attr.Value is int intValue)
                        {
                            builder.Set(attr.Key, intValue);
                        }
                        else if (attr.Value is double doubleValue)
                        {
                            builder.Set(attr.Key, doubleValue);
                        }
                        else
                        {
                            // Default to string representation for other types
                            builder.Set(attr.Key, attr.Value.ToString() ?? string.Empty);
                        }
                    }
                }
            }
            
            return builder.Build();
        }
    }
    
    #endregion
    
} 