

using Shared.DapperCustomAttributes;

namespace Entities.Models;

public class Token
{
    public int Id { get; set; }
    [Column(Name = "user_id")]
    public int UserId { get; set; }
    [Column(Name = "refresh_token")]
    public string RefreshToken { get; set; }
    [Column(Name = "expiry_time")]
    public DateTime ExpiryTime { get; set; }
}