namespace HipAndClavicle.Models;

public class AdminSettings
{
    public int AdminSettingsId { get; set; }
    // Preferred Contact is an Enum located in ~/Data/
    public PreferredContact? ContactBy { get; set; } = default!;
    public bool AutoReply { get; set; }
    public string? AutoReplyMessage { get; set; } = "";
    public bool ShareContactInfo { get; set; }
    public bool PurchaseRequiredForView { get; set; } = true;
}

