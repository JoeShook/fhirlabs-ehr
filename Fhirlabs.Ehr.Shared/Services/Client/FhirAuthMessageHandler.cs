#region (c) 2024 Joseph Shook. All rights reserved.
// /*
//  Authors:
//     Joseph Shook   Joseph.Shook@Surescripts.com
// 
//  See LICENSE in the project root for license information.
// */
#endregion

using System.Net.Http.Headers;
using System.Web;
using Fhirlabs.Ehr.Rcl.Model;
using Google.Apis.Auth.OAuth2;
using LazyCache;
using Microsoft.Extensions.Options;
using Udap.Util.Extensions;

namespace Fhirlabs.Ehr.Rcl.Services.Client;
public class FhirAuthMessageHandler(IOptionsMonitor<FhirClientOptions> fhirClientOptions, IAppCache cache)
    : DelegatingHandler
{
    private readonly FhirClientOptions _fhirClientOptions = fhirClientOptions.CurrentValue;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        GoogleCredential CachedCredential() => LoadCredential();
        var googleCredential = cache.GetOrAdd(_fhirClientOptions.KeyLocation, CachedCredential);
        
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", 
            await googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync(cancellationToken: cancellationToken));

        return await base.SendAsync(request, cancellationToken);
    }

    private GoogleCredential LoadCredential()
    {
        return GoogleCredential
            .FromStream(File.Open(_fhirClientOptions.KeyLocation, FileMode.Open))
            .CreateScoped("https://www.googleapis.com/auth/cloud-healthcare");
    }
}
