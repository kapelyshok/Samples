namespace AtomicApps.Utils
{
    public static class StringExtensions
    {
        public static bool IsFirstLetterVowel(this string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return false;

            char first = char.ToLowerInvariant(word[0]);
            return first == 'a' || first == 'e' || first == 'i' || first == 'o' || first == 'u';
        }
        
        public static bool IsVowel(this char c)
        {
            char lower = char.ToLowerInvariant(c);
            return lower == 'a' || lower == 'e' || lower == 'i' || lower == 'o' || lower == 'u';
        }
    }
}
