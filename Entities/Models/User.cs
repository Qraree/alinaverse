using System.ComponentModel.DataAnnotations;

namespace Entities.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Password { get; set; }
}