namespace HipAndClavicle.Models.JunctionTables
{
    public class ListingColorJT
    {
        public int ListingColorJTId {  get; set; }
        public Color ListingColor { get; set; }
        public Listing Listing { get; set; }
    }
}
