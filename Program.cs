using MxFace.SDK.Fingerprint;
using MxFace.SDK.Fingerprint.Extensions;
using MxFace.SDK.Fingerprint.Extensions.Configuration;
using MxFace.SDK.Fingerprint.Interfaces;
using MxFace.SDK.Fingerprint.Sample.NET.Components;
using MxFace.SDK.Fingerprint.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.UseMxFaceFingerprintSDK(configure: config =>
{
    config.Settings = new BiometricConfigurationSettings();
});

builder.Services.AddHttpClient<DeviceService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:8034/");
});
builder.Services.AddHttpClient<FingerprintCapturingService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:8034/");
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<ICaptureService, FingerprintCapturingService>();
builder.Services.AddScoped<IDevice, DeviceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
