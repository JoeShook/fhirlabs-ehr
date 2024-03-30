using Fhirlabs.Ehr.Rcl.Services.Client;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fhirlabs.Ehr.Components.Account.Pages;

public partial class PractitionerEmailDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Inject] private IFhirService FhirService { get; set; } = null!;

    private int selectedRowNumber = -1;
    private MudTable<Practitioner> _table;
    private Bundle? currentPage = null;
    private string searchString = string.Empty;
    private string selectedEmail = string.Empty;
    private void Submit() => MudDialog.Close(DialogResult.Ok(selectedEmail));
    private void Cancel() => MudDialog.Cancel();

    private string SelectedRowClassFunc(Practitioner element, int rowNumber)
    {
        if (selectedRowNumber == rowNumber)
        {
            selectedRowNumber = -1;
            return string.Empty;
        }
        else if (_table.SelectedItem != null && _table.SelectedItem.Equals(element))
        {
            selectedRowNumber = rowNumber;
            selectedEmail = element.Telecom.Where(t => t.System == ContactPoint.ContactPointSystem.Email)
                .Select(t => t.Value).FirstOrDefault() ?? string.Empty;
            return "selected";
        }
        else
        {
            return string.Empty;
        }
    }

    public int RowsPerPage { get; set; } = 5;

    private void OnSearch(string text)
    {
        searchString = text;
        currentPage = null;
        _table.ReloadServerData();
    }

    private async Task<TableData<Practitioner>> Reload(TableState arg)
    {
        var bundle = await FhirService.GetPractitioners(currentPage, searchString);
        currentPage = bundle;
        var practitioners = bundle.Entry.Select(entry => entry.Resource).Cast<Practitioner>().ToList();

        return new TableData<Practitioner>() { TotalItems = bundle.Total.Value, Items = practitioners };
    }
}