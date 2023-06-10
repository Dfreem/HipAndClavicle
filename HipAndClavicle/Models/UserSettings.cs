namespace HipAndClavicle.Models;

public class UserSettings
{
    public int UserSettingsId { get; set; }
    // Preferred Contact is an Enum located in ~/Data/
    public PreferredContact? ContactBy { get; set; } = PreferredContact.InAppMessage;
    public bool AutoReply { get; set; }
    public string? AutoReplyMessage { get; set; } = "";
    public bool ShareContactInfo { get; set; }
    public bool PurchaseRequiredForView { get; set; } = true;
    public int DaysTillShipmentIsLate { get; set; } = 14;
    public AppUser User { get; set; } = default!;
}

