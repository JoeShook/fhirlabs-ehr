#region (c) 2024 Joseph Shook. All rights reserved.
// /*
//  Authors:
//     Joseph Shook   Joseph.Shook@Surescripts.com
// 
//  See LICENSE in the project root for license information.
// */
#endregion

using Fhirlabs.Ehr.Data;
using Fhirlabs.Ehr.Rcl.Services.Client;
using Microsoft.AspNetCore.Identity;

namespace Fhirlabs.Ehr.Services.Validators;
public class FhirPractitionerValidator : IUserValidator<ApplicationUser>
{
    private readonly IFhirService _fhirService;

    public FhirPractitionerValidator(IFhirService fhirService)
    {
        _fhirService = fhirService;
    }

    /// <summary>
    /// Validates the specified <paramref name="user" /> as an asynchronous operation.
    /// </summary>
    /// <param name="manager">The <see cref="T:Microsoft.AspNetCore.Identity.UserManager`1" /> that can be used to retrieve user properties.</param>
    /// <param name="user">The user to validate.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the validation operation.</returns>
    public async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        var practitioner = await _fhirService.GetPractitioner(user.Email);

        if (practitioner != null)
        {
            return IdentityResult.Success;
        }

        return IdentityResult.Failed(new IdentityError { Description = "The email is not recognized as an existing Practitioner." });
    }
}
