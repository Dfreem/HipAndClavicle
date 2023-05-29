namespace HipAndClavicle.Models;

public class Image
{
    public int ImageId { get; set; }
    public byte[] ImageData { get; set; } = default!;
    public int Width { get; set; } = 200;
    public int Height { get; set; } = 200;
}
