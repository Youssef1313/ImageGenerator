using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
using ImageGenerator;

// Output image is 96dpi.
// Use this website to convert cm dimensions to pixels under a given resolution (in dpi) https://www.pixelto.net/cm-to-px-converter
float mmpi = 25.4f;
int dpi = 150;
var bitmap = new Bitmap((int)(210 / mmpi * dpi), (int)(297 / mmpi * dpi));
bitmap.SetResolution(dpi, dpi);

var g = Graphics.FromImage(bitmap);

const int NumberOfPages = 10;
Directory.CreateDirectory("./data/text");

var fontNames = new[]
{
    "Times New Roman",
    //"Tahoma",
    //"Arial",
    //"Calibri",
};

var generator = new ArabicWordGenerator(@"C:\Users\PC\Desktop\ImageGenerator\ImageGenerator\ar_reviews_100k.txt");

var imageToAdd = new Bitmap("C:\\Users\\PC\\Documents\\My Web Sites\\WebSite1\\w-brand.png");

foreach (var fontName in fontNames)
{
    var fontSize = Random.Shared.Next(18, 30);

    var regularFont = new Font(fontName, fontSize);
    var titleFont = new Font(fontName, fontSize + 8, FontStyle.Bold);
    for (int pageIndex = 0; pageIndex < NumberOfPages; pageIndex++)
    {
        var model = new JsonModel();
        g.FillRectangle(Brushes.White, new(0, 0, bitmap.Width, bitmap.Height));
        var numberOfImagesToAdd = Random.Shared.Next(0, 3);
        Debug.Assert(numberOfImagesToAdd is 0 or 1 or 2, "Update below code if this assert failed!!!");
        var imageRects = Array.Empty<Rectangle>();
        if (numberOfImagesToAdd == 1)
        {
            var (xDestination, yDestination) = bitmap.GetRandomLocationForImageToPut(imageToAdd);
            bitmap.PutImage(imageToAdd, xDestination, yDestination);
            imageRects = new Rectangle[]
            {
                new(xDestination, yDestination, imageToAdd.Width, imageToAdd.Height),
            };
        }
        else if (numberOfImagesToAdd == 2)
        {
            var (xDestination1, yDestination1) = bitmap.GetRandomLocationForImageToPut(imageToAdd);
            bitmap.PutImage(imageToAdd, xDestination1, yDestination1);
            var oldRect = new Rectangle(xDestination1, yDestination1, imageToAdd.Width, imageToAdd.Height);
            var (xDestination2, yDestination2) = bitmap.GetRandomLocationForSecondImageToPut(imageToAdd, oldRect);
            bitmap.PutImage(imageToAdd, xDestination2, yDestination2);
            imageRects = new Rectangle[]
            {
                new(xDestination1, yDestination1, imageToAdd.Width, imageToAdd.Height),
                new(xDestination2, yDestination2, imageToAdd.Width, imageToAdd.Height),
            };
        }

        foreach (var imageRect in imageRects)
        {
            model.Objects.Add(new() { Category = Category.ImageId, BoundingBox = new float[] { imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height } });
        }

        var currentY = 0;
        // Hard coded for now.
        for (int lineIndex = 0; lineIndex < 40; lineIndex++)
        {
            var currentX = bitmap.Width;
            var lineKind = GetLineKind();
            var font = lineKind switch
            {
                LineKind.Text => regularFont,
                LineKind.Title => titleFont,
                _ => throw new InvalidOperationException()
            };

            var numberOfWords = GetNumberOfWords(lineKind);
            var words = Enumerable.Range(0, numberOfWords).Select((_, _) => generator.Generate()).ToArray();

            var stringFormat = new StringFormat();
            stringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;

            var lineText = string.Join(' ', words);
            var lineTextMeasure = g.MeasureString(lineText, font, bitmap.Width, stringFormat);

            if (currentY + lineTextMeasure.Height > bitmap.Height)
            {
                break;
            }

            var spaceMeasure = g.MeasureString(" ", font, bitmap.Width, stringFormat);
            //var pen = Pens.Red;
            foreach (var word in words)
            {
                var wordMeasure = g.MeasureString(word, font, bitmap.Width, stringFormat);
                var leftOfWord = currentX - wordMeasure.Width;
                currentX -= (int)(wordMeasure.Width + spaceMeasure.Width);
                var wordRect = new RectangleF(leftOfWord, currentY, wordMeasure.Width, wordMeasure.Height);
                if (leftOfWord < 0 || imageRects.Any(r => wordRect.IntersectsWith(r)))
                {
                    continue;
                }

                g.DrawString(word, font, Brushes.Black, leftOfWord , currentY);
                
                //g.DrawRectangle(pen, wordRect);
                model.Objects.Add(new() { Text = word, Category = Category.TextId, BoundingBox = new[] { wordRect.X, wordRect.Y, wordRect.Width, wordRect.Height } });
                
                
                //pen = pen == Pens.Red ? Pens.Blue : Pens.Red;
            }

            currentY += (int)(lineTextMeasure.Height + 10);
        }

        Directory.CreateDirectory($"./data/{fontName}");
        bitmap.Save($"./data/{fontName}/{pageIndex}.png");
        File.WriteAllText($"./data/{fontName}/{pageIndex}.json", JsonSerializer.Serialize(model));
    }
}

static int GetNumberOfWords(LineKind lineKind)
{
    return lineKind switch
    {
        LineKind.Title => Random.Shared.Next(3, 7),
        LineKind.Text => Random.Shared.Next(8, 10),
        _ => throw new ArgumentOutOfRangeException(nameof(lineKind)),
    };
}

static LineKind GetLineKind()
{
    if (Random.Shared.NextDouble() <= 0.1)
    {
        return LineKind.Title;
    }

    return LineKind.Text;
}

enum LineKind
{
    Text, Title
}