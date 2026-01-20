using System;

namespace Shell.Client.Models;

public class AppConfig
{
    public string BaseUrl { get; set; }
    public AuthenticationConfig Authentication { get; set; }
}

public class AuthenticationConfig
{
    public AwsOidcConfig AWS { get; set; }
}

public class AwsOidcConfig
{
    public string Authority { get; set; }
    public string ClientId { get; set; }
    public string RedirectUri { get; set; }
    public string PostLogoutRedirectUri { get; set; }
    public string ResponseType { get; set; }
    public List<string> DefaultScopes { get; set; }
    public string MetadataUrl { get; set; }
}
