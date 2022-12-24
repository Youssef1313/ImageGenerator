using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using ImageGenerator;

var gMeasure = Graphics.FromImage(new Bitmap(1, 1));
gMeasure.CompositingQuality = CompositingQuality.HighQuality;

var wordBuilder = new StringBuilder();
var random = new Random();
const int NumberOfWords = 5000;
Directory.CreateDirectory("./data/text");
var quality = CompositingQuality.HighQuality;

var fontNames = new[]
{
    "Times New Roman",
    "Tahoma",
    "Arial",
    "Calibri",
    "Arabic Typesetting", //hard font.
};

for (int i = 0; i < NumberOfWords; i++)
{
    var wordLength = random.Next(1, 17);
    for (int j = 0; j < wordLength; j++)
    {
        wordBuilder.Append(ArabicCharacterGenerator.Generate());
    }

    var fontSize = random.Next(12, 25);
    var fontName = fontNames[random.Next(0, fontNames.Length)];
    var font = new Font(fontName, fontSize);
    var text = wordBuilder.ToString();
    var textSize = gMeasure.MeasureString(text, font);
    var bitmap = new Bitmap((int)textSize.Width + 100, (int)textSize.Height + 50);
    var g = Graphics.FromImage(bitmap);
    g.CompositingQuality = quality;
    quality = quality == CompositingQuality.HighQuality ? CompositingQuality.HighSpeed : CompositingQuality.HighQuality;
    g.FillRectangle(Brushes.White, new(0, 0, bitmap.Width, bitmap.Height));
    g.DrawString(text, font, Brushes.Black, (bitmap.Width - textSize.Width) / 2, (bitmap.Height - textSize.Height) / 2);

    bitmap.Save($"./data/{text}.png");
    File.WriteAllText($"./data/text/{text}.txt", $"{fontSize},{fontName}");
    wordBuilder.Clear();
}
