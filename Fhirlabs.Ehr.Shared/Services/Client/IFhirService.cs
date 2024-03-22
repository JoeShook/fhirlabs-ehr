#region (c) 2024 Joseph Shook. All rights reserved.
// /*
//  Authors:
//     Joseph Shook   Joseph.Shook@Surescripts.com
// 
//  See LICENSE in the project root for license information.
// */
#endregion

using Fhirlabs.Ehr.Rcl.Model;
using Hl7.Fhir.Model;

namespace Fhirlabs.Ehr.Rcl.Services.Client;
public interface IFhirService
{
    Task<Bundle> GetPatientsAsync(Bundle? currentPage, PatientSearchModel model);

    Task<Bundle> MatchPatient(string parametersJson);

    System.Threading.Tasks.Task DeletePatientAsync(string location);
    Task<Practitioner?> GetPractitioner(string? userEmail);
}