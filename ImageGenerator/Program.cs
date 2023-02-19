using System.Drawing;
using ImageGenerator;

// Output image is 96dpi.
// Use this website to convert cm dimensions to pixels under a given resolution (in dpi) https://www.pixelto.net/cm-to-px-converter
float mmpi = 25.4f;
int dpi = 150;
var bitmap = new Bitmap((int)(210 / mmpi * dpi), (int)(297 / mmpi * dpi));
bitmap.SetResolution(dpi, dpi);

var g = Graphics.FromImage(bitmap);

var random = new Random();
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

foreach (var fontName in fontNames)
{
    var fontSize = random.Next(18, 30);

    var regularFont = new Font(fontName, fontSize);
    var titleFont = new Font(fontName, fontSize + 8, FontStyle.Bold);
    for (int pageIndex = 0; pageIndex < NumberOfPages; pageIndex++)
    {
        g.FillRectangle(Brushes.White, new(0, 0, bitmap.Width, bitmap.Height));
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
            var pen = Pens.Red;
            foreach (var word in words)
            {
                var wordMeasure = g.MeasureString(word, font, bitmap.Width, stringFormat);
                var leftOfWord = currentX - wordMeasure.Width;
                if (leftOfWord < 0)
                {
                    continue;
                }
                g.DrawString(word, font, Brushes.Black, leftOfWord , currentY);
                g.DrawRectangle(pen, new RectangleF(leftOfWord, currentY, wordMeasure.Width, wordMeasure.Height));
                currentX -= (int)(wordMeasure.Width + spaceMeasure.Width);
                
                pen = pen == Pens.Red ? Pens.Blue : Pens.Red;
            }

            currentY += (int)(lineTextMeasure.Height + 10);
        }

        Directory.CreateDirectory($"./data/{fontName}");
        bitmap.Save($"./data/{fontName}/{pageIndex}.png");
    }
}

int GetNumberOfWords(LineKind lineKind)
{
    return lineKind switch
    {
        LineKind.Title => random.Next(3, 7),
        LineKind.Text => random.Next(8, 10),
        _ => throw new ArgumentOutOfRangeException(nameof(lineKind)),
    };
}

LineKind GetLineKind()
{
    if (random.NextDouble() <= 0.1)
    {
        return LineKind.Title;
    }

    return LineKind.Text;
}

enum LineKind
{
    Text, Title
}