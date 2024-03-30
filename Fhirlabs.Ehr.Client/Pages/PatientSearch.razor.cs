using Fhirlabs.Ehr.Rcl.Model;
using Fhirlabs.Ehr.Rcl.Services.Client;
using Fhirlabs.Ehr.Rcl.Shared;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Fhirlabs.Ehr.Client.Pages;

public partial class PatientSearch
{
    private MudForm form = null!;
    private MudTable<Patient> _table;
    private PatientSearchModel _model = new();
    private string? _outComeMessage;
    private string _selectedItemText = string.Empty;
    private Bundle? currentPage = null;


    [CascadingParameter]
    public CascadingAppState AppState { get; set; } = null!;

    [Inject] private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private IFhirService FhirService { get; set; } = null!;

    [Inject]
    public required IOptionsMonitor<FhirClientOptions> FhirClientOptions { get; set; }

    public int RowsPerPage { get; set; } = 5;


    private async Task Search()
    {
        currentPage = null;
        _table.CurrentPage = 0;
        _model.RowsPerPage = RowsPerPage;
        await _table.ReloadServerData();
    }

    private async Task Get()
    {
        _model.GetResource = true;

        await Search();
    }

    private void Cancel()
    {
        _model = new();
    }


    private async Task<TableData<Patient>> Reload(TableState state)
    {
        var bundle = await FhirService.GetPatientsAsync(currentPage, _model);

        currentPage = bundle;

        _model.NextLink = bundle.NextLink;
        _model.Page = state.Page;

        var patients = bundle.Entry.Select(entry => entry.Resource).Cast<Patient>().ToList();

        return new TableData<Patient>() { TotalItems = bundle.Total.Value, Items = patients };
    }

    private async Task DeletePatient(Patient patient)
    {
        if (await Js.InvokeAsync<bool>("confirm", $"Do you want to delete the Patient Record?"))
        {
            try
            {
                await FhirService.DeletePatientAsync($"{patient.TypeName}/{patient.Id}");
                currentPage = null;
                await _table.ReloadServerData();
            }
            catch (Exception e)
            {
                _outComeMessage = e.Message;
            }
        }
    }

    private void OnRowClick(TableRowClickEventArgs<Patient> args)
    {
        _selectedItemText = new FhirJsonSerializer(new SerializerSettings { Pretty = true })
            .SerializeToString(args.Item);
    }

    private int selectedRowNumber = -1;

    private string SelectedRowClassFunc(Patient patient, int rowNumber)
    {
        if (selectedRowNumber == rowNumber)
        {
            selectedRowNumber = -1;
            return string.Empty;
        }
        else if (_table.SelectedItem != null && _table.SelectedItem.Equals(patient))
        {
            selectedRowNumber = rowNumber;
            return "selected";
        }
        else
        {
            return string.Empty;
        }
    }
}