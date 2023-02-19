using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using ImageGenerator;

var gMeasure = Graphics.FromImage(new Bitmap(1, 1));
gMeasure.CompositingQuality = CompositingQuality.HighQuality;

var random = new Random();
const int NumberOfImages = 5;
Directory.CreateDirectory("./data/text");
var quality = CompositingQuality.HighQuality;

var fontNames = new[]
{
    "Times New Roman",
    "Tahoma",
    // "Times New Roman",
    "Times New Roman",
    "Arial",
    // "Calibri",
    // "Arabic Typesetting", //hard font.
};

var generator = new ArabicCharacterGenerator(@"F:\dotnet\ImageGenerator\ar_reviews_100k.txt");

for (int i = 0; i < NumberOfImages; i++)
{

    var wordLengthMin = 3;
    var wordLengthMax = 3;
    var wordLength = random.Next(wordLengthMin, wordLengthMax + 1);
    
    var fontSize = random.Next(18, 30);
    var fontName = fontNames[random.Next(0, fontNames.Length)];
    var font = new Font(fontName, fontSize);
    var text = string.Join(' ', Enumerable.Range(0, wordLength).Select((_, _) => generator.Generate()));
    var textSize = gMeasure.MeasureString(text, font);
    var bitmap = new Bitmap((int)textSize.Width + random.Next(0, 100), (int)textSize.Height + random.Next(0, 100));
    var g = Graphics.FromImage(bitmap);
    g.CompositingQuality = quality;
    quality = quality == CompositingQuality.HighQuality ? CompositingQuality.HighSpeed : CompositingQuality.HighQuality;
    g.FillRectangle(Brushes.White, new(0, 0, bitmap.Width, bitmap.Height));
    g.DrawString(text, font, Brushes.Black, random.Next(0, (int)(bitmap.Width - textSize.Width)) + 1, random.Next(0, (int)(bitmap.Height - textSize.Height)) + 1);
    // Console.WriteLine(text);
    bitmap.Save($"./data/{text}.png");
    File.WriteAllText($"./data/text/{text}.txt", $"{fontSize},{fontName}");
}
