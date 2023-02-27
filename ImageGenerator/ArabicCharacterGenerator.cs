namespace ImageGenerator;

internal sealed class ArabicWordGenerator
{
    private readonly static Random s_random = new();
    private readonly string[] _words;

    public ArabicWordGenerator(string fileName)
    {
        _words = File.ReadAllLines(fileName);
    }

    public string Generate()
    {
        return _words[s_random.Next(0, _words.Length)];
    }
}
