using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp;
using Client;
using MudBlazor.Services;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var services = builder.Services;
services.AddRefitClient<IBookClient>(new RefitSettings()
{
    ContentSerializer = new SystemTextJsonContentSerializer(),
    CollectionFormat = CollectionFormat.Multi
}).ConfigureHttpClient(x => x.BaseAddress = new Uri("http://localhost:5022"));
services.AddMudServices();

await builder.Build().RunAsync();