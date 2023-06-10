
using HipAndClavicle.Models.JunctionTables;

namespace HipAndClavicle.Models;

public class Listing
{
    public int ListingId { get; set; }

    //public Image? SingleImage { get; set; }
    public string ListingTitle { get; set; } = default!;
    public string ListingDescription { get; set;} = default!;
    public int? ListItemId { get; set; }
    public List<ListingItem> Items { get; set; } = new();

}
