using OpenFeature;

namespace GrowthBook.OpenFeature;

/// <summary>
/// Extension methods for the GrowthBook OpenFeature provider
/// </summary>
public static class ProviderExtensions
{
    /// <summary>
    /// Sets the GrowthBook provider as the default provider for OpenFeature
    /// </summary>
    /// <param name="api">The OpenFeature API instance</param>
    /// <param name="clientKey">GrowthBook client key</param>
    /// <param name="hostUrl">GrowthBook host URL</param>
    /// <param name="decryptionKey">GrowthBook encryption key (optional)</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static Task UseGrowthBookProvider(this global::OpenFeature.Api api, string apiKey, string clientKey, string hostUrl, string? decryptionKey = null)
    {
        var provider = new GrowthBookProvider(clientKey, hostUrl, decryptionKey);
        return api.SetProviderAsync(provider);
    }

    /// <summary>
    /// Sets the GrowthBook provider as the provider for a specific domain in OpenFeature
    /// </summary>
    /// <param name="api">The OpenFeature API instance</param>
    /// <param name="domain">The domain name</param>
    /// <param name="clientKey">GrowthBook client key</param>
    /// <param name="hostUrl">GrowthBook host URL</param>
    /// <param name="decryptionKey">GrowthBook encryption key (optional)</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static Task UseGrowthBookProvider(this global::OpenFeature.Api api, string domain, string apiKey, string clientKey, string hostUrl, string? decryptionKey = null)
    {
        var provider = new GrowthBookProvider(clientKey, hostUrl, decryptionKey);
        return api.SetProviderAsync(domain, provider);
    }

    /// <summary>
    /// Sets the GrowthBook provider as the default provider for OpenFeature using an existing GrowthBook instance
    /// </summary>
    /// <param name="api">The OpenFeature API instance</param>
    /// <param name="growthBookSdk">An initialized GrowthBook instance</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static Task UseGrowthBookProvider(this global::OpenFeature.Api api, global::GrowthBook.GrowthBook growthBookSdk)
    {
        var provider = new GrowthBookProvider(growthBookSdk);
        return api.SetProviderAsync(provider);
    }

    /// <summary>
    /// Sets the GrowthBook provider as the provider for a specific domain in OpenFeature using an existing GrowthBook instance
    /// </summary>
    /// <param name="api">The OpenFeature API instance</param>
    /// <param name="domain">The domain name</param>
    /// <param name="growthBookSdk">An initialized GrowthBook instance</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static Task UseGrowthBookProvider(this global::OpenFeature.Api api, string domain, global::GrowthBook.GrowthBook growthBookSdk)
    {
        var provider = new GrowthBookProvider(growthBookSdk);
        return api.SetProviderAsync(domain, provider);
    }
} 