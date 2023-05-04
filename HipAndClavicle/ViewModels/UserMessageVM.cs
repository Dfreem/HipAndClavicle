namespace HipAndClavicle.Models;

public class UserMessageVM
{
    public int UserMessageId { get; set; }
    public DateTime DateSent { get; } = DateTime.Now;
    public bool Read { get; set; }

    public string? Response { get; set; }

    //Added new columns 'Email & Contact' 4-18-23
    public string Email { get; set; }
    public string? Number { get; set; }


}