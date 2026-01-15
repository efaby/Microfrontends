using Microsoft.AspNetCore.Components.WebAssembly.Server;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// SHELL
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// PRODUCTS MFE
app.UseBlazorFrameworkFiles("/products");
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/products"
});

// ORDERS MFE
app.UseBlazorFrameworkFiles("/orders");
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/orders"
});

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapFallbackToFile("/", "index.html");
    endpoints.MapFallbackToFile("/products/{*path:nonfile}", "products/index.html");
    endpoints.MapFallbackToFile("/orders/{*path:nonfile}", "orders/index.html");
});

app.Run();