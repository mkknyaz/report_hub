using Exadel.ReportHub.Blazor.UI;
using Exadel.ReportHub.Blazor.UI.Components;
using Exadel.ReportHub.Blazor.UI.Handlers;
using Exadel.ReportHub.Blazor.UI.Services;
using Exadel.ReportHub.Blazor.UI.Services.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

var apiUrl = builder.Configuration["ReportHubApiUrl"];
ArgumentException.ThrowIfNullOrEmpty(apiUrl);

builder.Services.AddHttpClient("auth", client =>
{
	client.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient("api", client =>
	{
		client.BaseAddress = new Uri(apiUrl);
	})
	.AddHttpMessageHandler<TokenAuthorizationHandler>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/login";
	});

builder.Services.AddAuthorization();

builder.Services.AddSession();
builder.Services.AddScoped<IAuthService, AuthenticationService>();
builder.Services.AddTransient<TokenAuthorizationHandler>();
builder.Services.AddSingleton<IUserStateService, UserStateService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<IClientsService, ClientsService>();
builder.Services.AddScoped<IItemsService, ItemsService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IInvoicesService, InvoicesService>();
builder.Services.AddScoped<ICustomersService, CustomersService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();