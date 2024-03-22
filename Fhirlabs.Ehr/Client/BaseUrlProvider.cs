#region (c) 2024 Joseph Shook. All rights reserved.
// /*
//  Authors:
//     Joseph Shook   Joseph.Shook@Surescripts.com
// 
//  See LICENSE in the project root for license information.
// */
#endregion

using Fhirlabs.Ehr.Rcl.Model;
using Fhirlabs.Ehr.Rcl.Services.Client;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Options;

namespace Fhirlabs.Ehr.Client;

public class BaseUrlProvider : IBaseUrlProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptionsMonitor<FhirClientOptions> _options;

    public BaseUrlProvider(IHttpContextAccessor httpContextAccessor, IOptionsMonitor<FhirClientOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options;
    }
    
    public Uri GetBaseUrl()
    {
        string baseUrl;
        try
        {
            baseUrl = _httpContextAccessor.HttpContext?.Session.GetString(Constants.BASE_URL)
                      ?? _options.CurrentValue.BaseUrl;
        }
        catch
        {
            // session not available at startup.  AddIdentityCore links FhirPractitionerValidator to the User Validator collection 
            // which depends on FhirService and in turn this class.
            baseUrl = _options.CurrentValue.BaseUrl;
        }
        
        return new Uri(baseUrl.EnsureEndsWith("/"));
    }
}
