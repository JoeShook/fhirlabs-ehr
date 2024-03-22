#region (c) 2024 Joseph Shook. All rights reserved.
// /*
//  Authors:
//     Joseph Shook   Joseph.Shook@Surescripts.com
// 
//  See LICENSE in the project root for license information.
// */
#endregion

using Google.Apis.Auth.OAuth2;
using LazyCache;

namespace Fhirlabs.Ehr;


public class ServiceAccountCredentialCache
{
    private readonly IAppCache _cache;
    
    public ServiceAccountCredentialCache()
    {
        _cache = new CachingService();
    }
    public Task<string> GetAccessTokenAsync(string path, params string[] scopes)
    {
        GoogleCredential CachedCredential() => LoadCredential(path, scopes);

        var googleCredential = _cache.GetOrAdd(path, CachedCredential);

        return googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();
    }

    private GoogleCredential LoadCredential(string path, params string[] scopes)
    {
        return GoogleCredential
            .FromStream(File.Open(path, FileMode.Open))
            .CreateScoped(scopes);
    }
}