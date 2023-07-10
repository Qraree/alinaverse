namespace Entities.ConfigurationModels;

public class JwtConfiguration
{
    public string SecretKey { get; set; }
    public int LifeSpan { get; set; }
    public int RefreshTokenLifeSpan { get; set; }
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
}