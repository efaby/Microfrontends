using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.WebAssembly.Server;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// 🔐 AUTH
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "__Host-auth";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddOpenIdConnect(options =>
{
    options.Authority = "https://cognito-idp.us-east-2.amazonaws.com/us-east-2_vKyR3m62e";
    options.ClientId = "3eeoatinh81guqpugrellkortg";
    options.ClientSecret = "1qmniup7jush3sc49k984cvqvrvo8d258boflrsosm1djpa2jod0";
    options.ResponseType = "code";

    options.Scope.Add("openid");
    options.Scope.Add("email");
    options.Scope.Add("profile");

    options.CallbackPath = "/signin-oidc";
    options.SignedOutCallbackPath = "/signout-callback-oidc";

    options.TokenValidationParameters = new()
    {
        NameClaimType = "cognito:username",
        RoleClaimType = "cognito:groups"
    };

    options.SaveTokens = true;
});

// 🔁 Para NGINX / ALB
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

builder.Services.AddAuthorization();

var app = builder.Build();

// 🔁 IMPORTANTE: antes de todo
app.UseForwardedHeaders();

app.UseRouting();

// 🔐 AUTH PIPELINE
app.UseAuthentication();
app.UseAuthorization();

// ==== TU CÓDIGO (sin tocar lógica) ====

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



// 🔐 ENDPOINTS AUTH

app.MapGet("/login", (HttpContext ctx) =>
{
    return Results.Challenge(
        new AuthenticationProperties { RedirectUri = "/" },
        new[] { OpenIdConnectDefaults.AuthenticationScheme }
    );
});

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
});

app.MapGet("/auth/me", (HttpContext ctx) =>
{
    if (!ctx.User.Identity?.IsAuthenticated ?? true)
        return Results.Unauthorized();
    Console.WriteLine($"Username: {ctx.User.FindFirst("cognito:username")?.Value}");
    Console.WriteLine($"Email: {ctx.User.FindFirst(ClaimTypes.Email)?.Value}");
    return Results.Ok(new
    {
        name = ctx.User.FindFirst("cognito:username")?.Value,
        email = ctx.User.FindFirst(ClaimTypes.Email)?.Value,
        subject = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
    });
});


// FALLBACKS
app.MapFallbackToFile("/{*path:nonfile}", "index.html");
app.MapFallbackToFile("/products/{*path:nonfile}", "products/index.html");
app.MapFallbackToFile("/orders/{*path:nonfile}", "orders/index.html");

app.Run();
