namespace Entities.Models;

public class Tweet
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public DateTime DateTime { get; set; }
    public string Text { get; set; }
}