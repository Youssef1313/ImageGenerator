namespace ImageGenerator;

internal static class ArabicCharacterGenerator
{
    private readonly static Random s_random = new();
    private readonly static char[] s_arabicCharacters = new[]
    {
        'ا', 'أ', 'إ', 'ب', 'ت', 'ث' , 'ج' ,'ح','خ', 'د', 'ذ' , 'ر' ,'ز','س', 'ش', 'ص' , 'ض' ,'ط','ظ', 'ع', 'غ' , 'ف' ,'ق','ك', 'ل', 'م' , 'ن' ,'ه','و', 'ي', 'ى', 'ء'
    };

    public static char Generate()
    {
        var randomLetterIndex = s_random.Next(s_arabicCharacters.Length);
        return s_arabicCharacters[randomLetterIndex];
    }
}
