using System.Drawing;

namespace ImageGenerator;

internal static class Extensions
{
    public static void PutImage(this Bitmap destination, Bitmap source, int xDestination, int yDestination)
    {
        for (int i = 0; i < source.Width; i++)
        {
            for (int j = 0; j < source.Height; j++)
            {
                var currentPixel = source.GetPixel(i, j);
                destination.SetPixel(xDestination + i, yDestination + j, currentPixel);
            }
        }
    }

    public static (int X, int Y) GetRandomLocationForImageToPut(this Bitmap destination, Bitmap source)
    {
        return (X: Random.Shared.Next(0, destination.Width - source.Width), Y: Random.Shared.Next(0, destination.Height - source.Height));
    }

    public static (int X, int Y) GetRandomLocationForSecondImageToPut(this Bitmap destination, Bitmap source, Rectangle oldRect)
    {
        while (true)
        {
            var (x, y) = destination.GetRandomLocationForImageToPut(source);
            var newRect = new Rectangle(x, y, source.Width, source.Height);
            if (!oldRect.IntersectsWith(newRect))
            {
                return (x, y);
            }
        }
    }
}
