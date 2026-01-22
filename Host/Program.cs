using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// 🔐 AUTH
var authConfig = builder.Configuration.GetSection("Auth");

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
    if (!builder.Environment.IsDevelopment())
    {
        options.Cookie.Domain = ".midominio.com";
    }
})
.AddOpenIdConnect(options =>
{
    options.Authority = authConfig["Authority"];
    options.ClientId = authConfig["ClientId"];
    options.ClientSecret = authConfig["ClientSecret"];
    options.ResponseType = "code";

    options.Scope.Add("openid");
    options.Scope.Add("email");
    options.Scope.Add("profile");

    options.CallbackPath = authConfig["CallbackPath"];
    options.SignedOutCallbackPath = authConfig["SignedOutCallbackPath"];
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

if (app.Environment.IsDevelopment())
{
    app.MapGet("/auth/dev-login", async (HttpContext ctx) =>
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "dev-user"),
            new Claim(ClaimTypes.Email, "dev@local"),
            new Claim("cognito:username", "dev-user")
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await ctx.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return Results.Redirect("/");
    });
}


// 🔐 ENDPOINTS AUTH

app.MapGet("/auth/login", (HttpContext ctx) =>
{
    return Results.Challenge(
        new AuthenticationProperties { RedirectUri = "/" },
        new[] { OpenIdConnectDefaults.AuthenticationScheme }
    );
});

app.MapGet("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

    return Results.Redirect("/logged-out");
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

app.Run();
