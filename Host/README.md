# Blazor WebAssembly Microfrontend Integration Guide

This guide outlines the steps required to add and configure a new **Customers Client** microfrontend (MFE) within a Blazor WebAssembly host environment.

Create the project and add it to your solution via the CLI:

```bash
dotnet new blazorwasm -n Customers.Client
dotnet sln add Customers.Client
```

#### Customers - Index.html
Update the base path and global assets in `Customers.Client/wwwroot/index.html`:

* **Update Base Path:** `<base href="/customers/" />`
* **Add Styles & Scripts:**
```html
<link href="_content/Microfrontends.Shared.UI/css/shared.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
```

---

#### Customers - Layout Implementation (App.razor)
```razor
<AppShell AppName="Customers">
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="@typeof(SharedMainLayout)" />
            <FocusOnNavigate RouteData="@routeData" Selector="h1" />
        </Found>
    </Router>
</AppShell>
```

#### Customers - Shared Imports (_Imports.razor)
```razor
@using Microfrontends.Shared.UI
@using Microfrontends.Shared.UI.Layout
@using Microfrontends.Shared.UI.Components
```

> **Note:** Delete the default `Layout` folder and keep only `Home.razor` in the Pages folder.

---

#### Customers - Program.cs
```csharp
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(nav.BaseUri + "../")
    };
});

builder.Services.AddSharedAuth();
```

#### Customers - Project File (Customers.Client.csproj)
```xml
<PropertyGroup>
    <StaticWebAssetBasePath>customers</StaticWebAssetBasePath>
    <StaticWebAssetFingerprintingEnabled>false</StaticWebAssetFingerprintingEnabled>
    <StaticWebAssetsFingerprintContent>false</StaticWebAssetsFingerprintContent>
</PropertyGroup>

<ItemGroup>
    <ProjectReference Include="..\\Shared.Auth\\Shared.Auth.csproj" />
    <ProjectReference Include="..\\Shared\\Microfrontends.Shared.Core\\Microfrontends.Shared.Core.csproj" />
    <ProjectReference Include="..\\Shared\\Microfrontends.Shared.UI\\Microfrontends.Shared.UI.csproj" />
</ItemGroup>
```

---
#### Shared.UI - SideMenu

Add this to `SideMenu.razor` in Shared/Microfrontends.Sahred.UI/Layout:

```razor
<li class="nav-item">
    <a class="nav-link text-white" @onclick="@(() => GoTo("/customers/"))"> Customers</a>
</li>
```