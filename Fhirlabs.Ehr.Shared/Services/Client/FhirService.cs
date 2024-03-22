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
using Hl7.Fhir.Rest;
using Task = System.Threading.Tasks.Task;

namespace Fhirlabs.Ehr.Rcl.Services.Client;
public class FhirService : IFhirService
{
    private readonly FhirClientWithUrlProvider _fhirClient;

    public FhirService(FhirClientWithUrlProvider fhirClient)
    {
        _fhirClient = fhirClient;
    }
    
    public async Task<Bundle> GetPatientsAsync(Bundle? currentPage, PatientSearchModel model)
    {
        if (currentPage == null)
        {
            return await _fhirClient.SearchAsync<Patient>(BuildSearchParams(model).OrderBy("given").LimitTo(model.RowsPerPage));
        }
       
        return await _fhirClient.ContinueAsync(currentPage);
    }

    public Task<Bundle> MatchPatient(string parametersJson)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePatientAsync(string location)
    {
        await _fhirClient.DeleteAsync(location);
    }

    public async Task<Practitioner?> GetPractitioner(string? userEmail)
    {
        var bundle = await _fhirClient.SearchAsync<Practitioner>(new SearchParams().Add("email", userEmail));
        return bundle.Entry.Select(e => e.Resource as Practitioner).FirstOrDefault();
    }

    private SearchParams BuildSearchParams(PatientSearchModel model)
    {
        var searchParams = new SearchParams();

        if (!string.IsNullOrEmpty(model.Id))
        {
            searchParams.Add("_id", model.Id);
        }

        if (!string.IsNullOrEmpty(model.Identifier))
        {
            searchParams.Add("identifier", model.Identifier);
        }

        if (!string.IsNullOrEmpty(model.Family))
        {
            searchParams.Add("family", model.Family);
        }

        if (!string.IsNullOrEmpty(model.Given))
        {
            searchParams.Add("given", model.Given);
        }

        if (!string.IsNullOrEmpty(model.Name))
        {
            searchParams.Add("name", model.Name);
        }

        if (model.BirthDate.HasValue)
        {
            searchParams.Add("birthdate", model.BirthDate.Value.ToString("yyyy-MM-dd"));
        }

        searchParams.Add("_count", model.RowsPerPage.ToString());

        return searchParams;
    }
}
