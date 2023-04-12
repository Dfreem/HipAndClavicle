namespace HipAndClavicle.Models;

public class UserMessage
{
    public int UserMessageId { get; set; }
    [Required]
    public AppUser Sender { get; set; } = default!;
    public DateTime DateSent { get; } = DateTime.Now;
    public bool Read { get; set; }
    public string? Response { get; set; }
    public int? OrderId { get; set; }
}