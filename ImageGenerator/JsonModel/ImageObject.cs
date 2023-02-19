namespace ImageGenerator;

public class ImageObject
{
    public int Category { get; set; }

    // topLeftX, topLeftY, width, height
    public float[] BoundingBox { get; set; }

    public string? Text { get; set; }
}