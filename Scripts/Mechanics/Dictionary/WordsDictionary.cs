using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace AtomicApps.Mechanics.Gameplay.Dictionary
{
    public class WordsDictionary
    {
        private readonly HashSet<string> _wordSet;
        private readonly List<string> _wordsByLengthDesc;

        public WordsDictionary(IEnumerable<string> words)
        {
            _wordSet = new HashSet<string>(words.Select(w => w.ToLowerInvariant()));
            _wordsByLengthDesc = _wordSet.OrderByDescending(w => w.Length).ToList();
        }

        public bool Contains(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            
            if (!input.Contains("%"))
            {
                return _wordSet.Contains(input.ToLowerInvariant());
            }

            string lowerInput = input.ToLowerInvariant();
            Dictionary<string, int> letterFreq = new();
            int wildcardCount = 0;

            foreach (char c in lowerInput)
            {
                if (c == '%')
                {
                    wildcardCount++;
                    continue;
                }

                string letter = c.ToString();
                if (!letterFreq.TryAdd(letter, 1))
                {
                    letterFreq[letter]++;
                }
            }

            foreach (string word in _wordSet)
            {
                if (word.Length > lowerInput.Length) continue;

                if (CanBuildWordFromChunksWithWildcards(word, letterFreq, wildcardCount))
                {
                    return true;
                }
            }

            return false;
        }

        public UniTask<string> GetLongestWordFromLettersAsync(IEnumerable<string> letters, int maxLettersCount)
        {
            return UniTask.Run(() =>
            {
                Dictionary<string, int> inputLetterFreq = new Dictionary<string, int>();
                int wildcardCount = 0;

                foreach (string letter in letters)
                {
                    string lower = letter.ToLowerInvariant();
                    if (string.IsNullOrWhiteSpace(lower)) continue;

                    if (lower == "%") wildcardCount++;
                    else if (!inputLetterFreq.TryAdd(lower, 1)) inputLetterFreq[lower]++;
                }

                foreach (string word in _wordsByLengthDesc)
                {
                    if (word.Length > maxLettersCount) continue;

                    if (CanBuildWordFromChunksWithWildcards(word, inputLetterFreq, wildcardCount))
                        return word;
                }

                return null;
            });
        }

        private bool CanBuildWordFromChunksWithWildcards(string word, Dictionary<string, int> availableChunks, int remainingWildcards)
        {
            return TryBuildWithWildcards(word, availableChunks, new Dictionary<string, int>(), remainingWildcards);
        }

        private bool TryBuildWithWildcards(string remaining, Dictionary<string, int> available, Dictionary<string, int> used, int wildcardsLeft)
        {
            if (remaining.Length == 0) return true;

            foreach (var kvp in available)
            {
                string chunk = kvp.Key;
                int availableCount = kvp.Value;
                int usedCount = used.TryGetValue(chunk, out int val) ? val : 0;

                if (usedCount >= availableCount) continue;
                if (!remaining.StartsWith(chunk)) continue;

                used[chunk] = usedCount + 1;

                if (TryBuildWithWildcards(remaining.Substring(chunk.Length), available, used, wildcardsLeft))
                    return true;

                used[chunk] = usedCount; // backtrack
            }

            if (wildcardsLeft > 0 && remaining.Length >= 1)
                return TryBuildWithWildcards(remaining.Substring(1), available, used, wildcardsLeft - 1);

            return false;
        }
    }
}
