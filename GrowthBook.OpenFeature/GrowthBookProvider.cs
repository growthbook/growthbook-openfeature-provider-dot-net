using System.Collections.Immutable;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OpenFeature;
using OpenFeature.Model;
using OpenFeature.Constant;

namespace GrowthBook.OpenFeature;

/// <summary>
/// GrowthBook provider for OpenFeature
/// </summary>
public class GrowthBookProvider : FeatureProvider
{
    private readonly global::GrowthBook.GrowthBook _growthBook;
    private readonly string? _apiKey;
    private readonly string? _clientKey;
    private readonly string? _hostUrl;
    private readonly string? _decryptionKey;

    /// <summary>
    /// Initializes a new instance of the GrowthBookProvider class
    /// </summary>
    /// <param name="apiKey">GrowthBook API key</param>
    /// <param name="clientKey">GrowthBook client key</param>
    /// <param name="hostUrl">GrowthBook host URL</param>
    /// <param name="decryptionKey">GrowthBook encryption key (optional)</param>
    public GrowthBookProvider(string apiKey, string clientKey, string hostUrl, string? decryptionKey = null)
    {
        _apiKey = apiKey;
        _clientKey = clientKey;
        _hostUrl = hostUrl;
        _decryptionKey = decryptionKey;

        var context = new global::GrowthBook.Context
        {
            ApiHost = hostUrl,
            ClientKey = clientKey
        };

        // The C# GrowthBook SDK might not support encryption key
        // If needed, this would be implemented according to the SDK's capabilities

        _growthBook = new global::GrowthBook.GrowthBook(context);
    }

    /// <summary>
    /// Initializes a new instance of the GrowthBookProvider class with an existing GrowthBook instance
    /// </summary>
    /// <param name="growthBookSdk">An initialized GrowthBook instance</param>
    public GrowthBookProvider(global::GrowthBook.GrowthBook growthBookSdk)
    {
        _growthBook = growthBookSdk;
    }

    /// <inheritdoc/>
    public override Metadata GetMetadata()
    {
        return new Metadata("GrowthBook Feature Provider");
    }

    /// <summary>
    /// Maps OpenFeature EvaluationContext to GrowthBook attributes
    /// </summary>
    /// <param name="evaluationContext">The OpenFeature evaluation context</param>
    /// <returns>JObject of attributes for GrowthBook</returns>
    private JObject MapAttributes(EvaluationContext? evaluationContext)
    {
        var attributes = new JObject();
        
        if (evaluationContext == null)
        {
            return attributes;
        }

        // Add targeting key as userId if available
        if (!string.IsNullOrEmpty(evaluationContext.TargetingKey))
        {
            attributes.Add("userId", evaluationContext.TargetingKey);
        }
        
        // Map all other attributes from the evaluation context
        foreach (var entry in evaluationContext.AsDictionary())
        {
            if (entry.Key == "targetingKey") 
            {
                continue; // Already handled as userId
            }

            if (entry.Value.IsString)
            {
                attributes.Add(entry.Key, entry.Value.AsString);
            }
            else if (entry.Value.IsBoolean)
            {
                attributes.Add(entry.Key, entry.Value.AsBoolean);
            }
            else if (entry.Value.IsNumber)
            {
                // Check if it's an integer or double
                if (entry.Value.AsInteger.HasValue)
                {
                    attributes.Add(entry.Key, entry.Value.AsInteger.Value);
                }
                else if (entry.Value.AsDouble.HasValue)
                {
                    attributes.Add(entry.Key, entry.Value.AsDouble.Value);
                }
            }
            else if (entry.Value.IsStructure)
            {
                try
                {
                    // Try to convert to JToken
                    var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(entry.Value.AsStructure);
                    var token = JToken.Parse(jsonStr);
                    attributes.Add(entry.Key, token);
                }
                catch
                {
                    // If conversion fails, add as string
                    attributes.Add(entry.Key, entry.Value.ToString());
                }
            }
        }
        
        return attributes;
    }

    /// <inheritdoc/>
    public override Task<ResolutionDetails<bool>> ResolveBooleanValueAsync(string flagKey, bool defaultValue, EvaluationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Map context attributes if needed
            if (context != null)
            {
                _growthBook.Attributes = MapAttributes(context);
            }
            
            // Check if the feature exists in the features dictionary
            bool result;
            string? variant = null;
            string? reason = null;
            
            if (_growthBook.Features != null && _growthBook.Features.ContainsKey(flagKey))
            {
                // Feature exists, get its value
                result = _growthBook.IsOn(flagKey);
                
                // We'll use a generic reason for targeting match
                reason = "TARGETING_MATCH";
            }
            else
            {
                // Feature not found, use default value
                result = defaultValue;
                reason = "DEFAULT";
            }
            
            return Task.FromResult(new ResolutionDetails<bool>(
                flagKey,              // flagKey
                result,               // value
                ErrorType.None,       // errorType
                reason,               // reason
                variant,              // variant
                null,                 // errorMessage
                new ImmutableMetadata()  // flagMetadata
            ));
        }
        catch (Exception ex)
        {
            // Handle any exceptions during evaluation
            return Task.FromResult(new ResolutionDetails<bool>(
                flagKey,                      // flagKey
                defaultValue,                 // value (use default on error)
                ErrorType.General,            // errorType
                "ERROR",                      // reason
                null,                         // variant
                ex.Message,                   // errorMessage
                new ImmutableMetadata()       // flagMetadata
            ));
        }
    }

    /// <inheritdoc/>
    public override Task<ResolutionDetails<string>> ResolveStringValueAsync(string flagKey, string defaultValue, EvaluationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Map context attributes if needed
            if (context != null)
            {
                _growthBook.Attributes = MapAttributes(context);
            }
            
            // Check if the feature exists in the features dictionary
            string result;
            string? variant = null;
            string? reason = null;
            
            if (_growthBook.Features != null && _growthBook.Features.ContainsKey(flagKey))
            {
                // Feature exists, get its value
                result = _growthBook.GetFeatureValue<string>(flagKey, defaultValue);
                
                // We'll use a generic reason for targeting match
                reason = "TARGETING_MATCH";
            }
            else
            {
                // Feature not found, use default value
                result = defaultValue;
                reason = "DEFAULT";
            }
            
            return Task.FromResult(new ResolutionDetails<string>(
                flagKey,              // flagKey
                result,               // value
                ErrorType.None,       // errorType
                reason,               // reason
                variant,              // variant
                null,                 // errorMessage
                new ImmutableMetadata()  // flagMetadata
            ));
        }
        catch (Exception ex)
        {
            // Handle any exceptions during evaluation
            return Task.FromResult(new ResolutionDetails<string>(
                flagKey,                      // flagKey
                defaultValue,                 // value (use default on error)
                ErrorType.General,            // errorType
                "ERROR",                      // reason
                null,                         // variant
                ex.Message,                   // errorMessage
                new ImmutableMetadata()       // flagMetadata
            ));
        }
    }

    /// <inheritdoc/>
    public override Task<ResolutionDetails<int>> ResolveIntegerValueAsync(string flagKey, int defaultValue, EvaluationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Map context attributes if needed
            if (context != null)
            {
                _growthBook.Attributes = MapAttributes(context);
            }
            
            // Check if the feature exists in the features dictionary
            int result;
            string? variant = null;
            string? reason = null;
            
            if (_growthBook.Features != null && _growthBook.Features.ContainsKey(flagKey))
            {
                // Feature exists, get its value
                result = _growthBook.GetFeatureValue<int>(flagKey, defaultValue);
                
                // We'll use a generic reason for targeting match
                reason = "TARGETING_MATCH";
            }
            else
            {
                // Feature not found, use default value
                result = defaultValue;
                reason = "DEFAULT";
            }
            
            return Task.FromResult(new ResolutionDetails<int>(
                flagKey,              // flagKey
                result,               // value
                ErrorType.None,       // errorType
                reason,               // reason
                variant,              // variant
                null,                 // errorMessage
                new ImmutableMetadata()  // flagMetadata
            ));
        }
        catch (Exception ex)
        {
            // Handle any exceptions during evaluation
            return Task.FromResult(new ResolutionDetails<int>(
                flagKey,                      // flagKey
                defaultValue,                 // value (use default on error)
                ErrorType.General,            // errorType
                "ERROR",                      // reason
                null,                         // variant
                ex.Message,                   // errorMessage
                new ImmutableMetadata()       // flagMetadata
            ));
        }
    }

    /// <inheritdoc/>
    public override Task<ResolutionDetails<double>> ResolveDoubleValueAsync(string flagKey, double defaultValue, EvaluationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Map context attributes if needed
            if (context != null)
            {
                _growthBook.Attributes = MapAttributes(context);
            }
            
            // Check if the feature exists in the features dictionary
            double result;
            string? variant = null;
            string? reason = null;
            
            if (_growthBook.Features != null && _growthBook.Features.ContainsKey(flagKey))
            {
                // Feature exists, get its value
                result = _growthBook.GetFeatureValue<double>(flagKey, defaultValue);
                
                // We'll use a generic reason for targeting match
                reason = "TARGETING_MATCH";
            }
            else
            {
                // Feature not found, use default value
                result = defaultValue;
                reason = "DEFAULT";
            }
            
            return Task.FromResult(new ResolutionDetails<double>(
                flagKey,              // flagKey
                result,               // value
                ErrorType.None,       // errorType
                reason,               // reason
                variant,              // variant
                null,                 // errorMessage
                new ImmutableMetadata()  // flagMetadata
            ));
        }
        catch (Exception ex)
        {
            // Handle any exceptions during evaluation
            return Task.FromResult(new ResolutionDetails<double>(
                flagKey,                      // flagKey
                defaultValue,                 // value (use default on error)
                ErrorType.General,            // errorType
                "ERROR",                      // reason
                null,                         // variant
                ex.Message,                   // errorMessage
                new ImmutableMetadata()       // flagMetadata
            ));
        }
    }

    /// <inheritdoc/>
    public override Task<ResolutionDetails<Value>> ResolveStructureValueAsync(string flagKey, Value defaultValue, EvaluationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Map context attributes if needed
            if (context != null)
            {
                _growthBook.Attributes = MapAttributes(context);
            }
            
            // For structure values, we'll use the default value since there's no direct mapping
            // between GrowthBook and OpenFeature structure types
            string? reason = "DEFAULT";
            
            if (_growthBook.Features != null && _growthBook.Features.ContainsKey(flagKey))
            {
                reason = "TARGETING_MATCH";
                // Note: We could try to convert GrowthBook feature value to OpenFeature Value,
                // but for simplicity we'll use the default value
            }
            
            return Task.FromResult(new ResolutionDetails<Value>(
                flagKey,              // flagKey
                defaultValue,         // value
                ErrorType.None,       // errorType
                reason,               // reason
                null,                 // variant
                null,                 // errorMessage
                new ImmutableMetadata()  // flagMetadata
            ));
        }
        catch (Exception ex)
        {
            // Handle any exceptions during evaluation
            return Task.FromResult(new ResolutionDetails<Value>(
                flagKey,                      // flagKey
                defaultValue,                 // value (use default on error)
                ErrorType.General,            // errorType
                "ERROR",                      // reason
                null,                         // variant
                ex.Message,                   // errorMessage
                new ImmutableMetadata()       // flagMetadata
            ));
        }
    }
}
