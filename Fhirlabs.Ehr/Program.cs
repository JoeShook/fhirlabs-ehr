#region (c) 2024 Joseph Shook. All rights reserved.
// /*
//  Authors:
//     Joseph Shook   Joseph.Shook@Surescripts.com
// 
//  See LICENSE in the project root for license information.
// */
#endregion

using Blazored.LocalStorage;
using Fhirlabs.Ehr.Client;
using Fhirlabs.Ehr.Components;
using Fhirlabs.Ehr.Components.Account;
using Fhirlabs.Ehr.Data;
using Fhirlabs.Ehr.Rcl.Model;
using Fhirlabs.Ehr.Rcl.Services;
using Fhirlabs.Ehr.Rcl.Services.Client;
using Hl7.Fhir.Rest;
using LazyCache;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddMudServices();


builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        // very lax so it is easy for demo.
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 3;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


// EHR Section

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.Name = ".FhirLabs.Ehr";
    options.Cookie.IsEssential = true;
});



builder.Services.Configure<FhirClientOptions>(builder.Configuration.GetSection("FhirClientOptions"));
builder.Services.AddTransient<FhirAuthMessageHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBaseUrlProvider, BaseUrlProvider>();
builder.Services.AddSingleton(new FhirClientSettings() { PreferredFormat = ResourceFormat.Json });
builder.Services.AddHttpClient<FhirClientWithUrlProvider>().AddHttpMessageHandler<FhirAuthMessageHandler>();

builder.Services.AddScoped<IFhirService, FhirService>();
builder.Services.AddSingleton<IAppCache, CachingService>();
builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

app.UseStatusCodePagesWithRedirects("/404");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseSession();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Fhirlabs.Ehr.Client._Imports).Assembly, typeof(IAppState).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
