using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ImageGenerator;

// Output image is 96dpi.
// Use this website to convert cm dimensions to pixels under a given resolution (in dpi) https://www.pixelto.net/cm-to-px-converter
const int WidthFor96Dpi = 794;
const int HeightFor96Dpi = 1134;
var bitmap = new Bitmap(width: WidthFor96Dpi, height: HeightFor96Dpi);
var g = Graphics.FromImage(bitmap);

var random = new Random();
const int NumberOfPages = 1;
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

        // one line. Hard coded for now. TODO: Increase
        for (int lineIndex = 0; lineIndex < 20; lineIndex++)
        {
            var lineKind = GetLineKind();
            var font = lineKind switch
            {
                LineKind.Text => regularFont,
                LineKind.Title => titleFont,
                _ => throw new InvalidOperationException()
            };

            var numberOfWords = GetNumberOfWords(lineKind);
            var words = Enumerable.Range(0, numberOfWords).Select((_, _) => generator.Generate()).ToArray();

            var lineText = string.Join(' ', words);
            var lineTextMeasure = g.MeasureString(lineText, font);

            var ranges = new List<CharacterRange>();
            var startRange = 0;
            foreach (var word in words)
            {
                ranges.Add(new CharacterRange(startRange, word.Length));
                startRange += word.Length;
                ranges.Add(new CharacterRange(startRange, 1));
                startRange++;
            }

            ranges.RemoveAt(ranges.Count - 1);

            var stringFormat = new StringFormat();
            stringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            stringFormat.SetMeasurableCharacterRanges(ranges.ToArray());
            var rect = new RectangleF(x: 0, y: currentY, width: bitmap.Width, height: lineTextMeasure.Height);
            var measuredRanges = g.MeasureCharacterRanges(lineText, font, rect, stringFormat);

            Debug.Assert(ranges.Count == measuredRanges.Length);
            var pen = Pens.Red;
            for (int wordCounter = 0; wordCounter < ranges.Count; wordCounter += 2)
            {
                var currentMeasured = measuredRanges[wordCounter];
                var currentRange = ranges[wordCounter];
                var currentWord = lineText.Substring(currentRange.First, currentRange.Length);
                var currentRect = currentMeasured.GetBounds(g);
                var gMeasure = g.MeasureString(currentWord, font);
                g.DrawString(currentWord, font, Brushes.Black, new RectangleF(currentRect.X, currentRect.Y, gMeasure.Width, gMeasure.Height), stringFormat);
                g.DrawRectangle(pen, currentRect);
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