using Microsoft.Extensions.Hosting;
using Projecteuler.Components;
using RazorConsole.Core;

var builder = Host.CreateApplicationBuilder(args);

Console.Clear();
builder.UseRazorConsole<App>();

await builder.Build().RunAsync();
