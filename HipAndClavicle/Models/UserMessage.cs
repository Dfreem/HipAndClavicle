namespace HipAndClavicle.Models;

public class UserMessage
{
    [Required]
    public AppUser Sender { get; set; } = default!;
    public DateTime DateSent { get; } = DateTime.Now;
    public bool Read { get; set; } = false;
    public string? Response { get; set; }
    public int? OrderId { get; set; }
}