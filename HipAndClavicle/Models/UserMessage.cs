namespace HipAndClavicle.Models;

public class UserMessage
{
    public int Id { get; set; }

    //[4-23-23]Commented out the 'Required' attirbute to remedy adding message to db issue.
    //[Required]
    public string? SenderUserName { get; set; }
    public AppUser? Sender { get; set; } = default!;

    public string? ReceiverUserName { get; set; }
    public AppUser? Receiver { get; set; } = default!;

    public DateTime DateSent { get; set; }
    public bool Read { get; set; }
    public string? Content { get; set; }
    public int? OrderId { get; set; }

    //Added new columns 'Email & Contact' 4-18-23
    public string? Email { get; set; }
    public string? Number { get; set; }


}