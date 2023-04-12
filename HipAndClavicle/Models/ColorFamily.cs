namespace HipAndClavicle.Models
{
    public class ColorFamily
    {
        public int ColorFamilyId { get; set; }
        public string ColorFamilyName { get; set; } = default!;
        public Color Color { get; set; } = default!;
    }
}
