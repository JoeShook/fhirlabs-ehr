#region (c) 2024 Joseph Shook. All rights reserved.
// /*
//  Authors:
//     Joseph Shook   Joseph.Shook@Surescripts.com
// 
//  See LICENSE in the project root for license information.
// */
#endregion

using Fhirlabs.Ehr.Rcl.Model.AppState;

namespace Fhirlabs.Ehr.Rcl.Services;
public class FhirlabsState : IAppState
{
    public PatientSearchPref? PatientSearchPref { get; set; }
    public DateTime LastStorageSaveTime { get; set; }
}
