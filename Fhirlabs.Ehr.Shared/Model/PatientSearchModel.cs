#region (c) 2024 Joseph Shook. All rights reserved.
// /*
//  Authors:
//     Joseph Shook   Joseph.Shook@Surescripts.com
// 
//  See LICENSE in the project root for license information.
// */
#endregion

using Hl7.Fhir.Model;

namespace Fhirlabs.Ehr.Rcl.Model;

public class PatientSearchModel
{
    public bool GetResource { get; set; }

    public string? Family { get; set; }
    public string? Given { get; set; }
    public string? Name { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Id { get; set; }
    public string? Identifier { get; set; }

    public int RowsPerPage { get; set; } = 5;
    public Uri? NextLink { get; set; }
    public int Page { get; set; }
}

public class PatientMatchModel
{
    public string? Family { get; set; }
    public string? Given { get; set; }
    public AdministrativeGender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }

    public string? Identifier { get; set; }

}

