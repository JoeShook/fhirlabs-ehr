#region (c) 2024 Joseph Shook. All rights reserved.
// /*
//  Authors:
//     Joseph Shook   Joseph.Shook@Surescripts.com
// 
//  See LICENSE in the project root for license information.
// */
#endregion

namespace Fhirlabs.Ehr.Rcl.Model;

[Serializable]
public class FhirClientOptions
{
    public required string KeyLocation { get; set; }
    public string BaseUrl { get; set; }
}
