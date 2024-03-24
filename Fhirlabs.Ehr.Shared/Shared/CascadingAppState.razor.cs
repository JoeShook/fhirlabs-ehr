#region (c) 2024 Joseph Shook. All rights reserved.
// /*
//  Authors:
//     Joseph Shook   Joseph.Shook@Surescripts.com
// 
//  See LICENSE in the project root for license information.
// */
#endregion

using System.Text.Json;
using Blazored.LocalStorage;
using Fhirlabs.Ehr.Rcl.Model.AppState;
using Fhirlabs.Ehr.Rcl.Services;
using Microsoft.AspNetCore.Components;

namespace Fhirlabs.Ehr.Rcl.Shared;

public partial class CascadingAppState : ComponentBase, IAppState
{
    private const string STORAGE_KEY = "FhirlabsEhr";

    private const int STORAGE_TIME_IN_SECONDS = 30;

    bool loaded;

    public DateTime LastStorageSaveTime { get; set; }

    [Inject]
    ILocalStorageService LocalStorage { get; set; } = null!;

    [Parameter] public required RenderFragment ChildContent { get; set; }

    private readonly List<EventCallback<FhirlabsStateArgs>> _callbacks
        = new List<EventCallback<FhirlabsStateArgs>>();

    // Each component that wants to be notified will register a callback
    public void RegisterCallback(EventCallback<FhirlabsStateArgs> callback)
    {
        if (!_callbacks.Contains(callback))
        {
            _callbacks.Add(callback);
        }
    }

    // We call this from our property setters
    private void NotifyPropertyChanged(FhirlabsStateArgs args)
    {
        foreach (var callback in _callbacks)
        {
            // Ignore exceptions due to dangling references
            try
            {
                // Invoke the callback
                callback.InvokeAsync(args);
            }
            catch
            {
                // ignored
            }
        }
    }
    
    private PatientSearchPref? _patientSearchPref;

    public PatientSearchPref PatientSearchPref
    {
        get { return _patientSearchPref ??= new PatientSearchPref(); }
        private set
        {
            _patientSearchPref = value;
            StateHasChanged();
            NotifyPropertyChanged(new(nameof(PatientSearchPref), value));
            new Task(async () =>
            {
                await Save();
            }).Start();
        }
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Load();
            loaded = true;
            StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        
    }

    public async Task Save()
    {
        if (!loaded) return;

        // set LastSaveTime
        LastStorageSaveTime = DateTime.Now;
        // serialize 
        var state = (IAppState)this;
        // save
        await LocalStorage.SetItemAsync<IAppState>(STORAGE_KEY, state);
    }

    public async Task Load()
    {
        try
        {
            var json = await LocalStorage.GetItemAsStringAsync(STORAGE_KEY);
            if (string.IsNullOrEmpty(json))
            {
                return;
            }
            var state = JsonSerializer.Deserialize<FhirlabsState>(json);
            if (state != null)
            {
                if (DateTime.Now.Subtract(state.LastStorageSaveTime).TotalSeconds <= STORAGE_TIME_IN_SECONDS)
                {
                    var t = typeof(IAppState);
                    var props = t.GetProperties();

                    foreach (var prop in props)
                    {
                        if (prop.Name != "LastStorageSaveTime")
                        {
                            var value = prop.GetValue(state);
                            prop.SetValue(this, value, null);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}