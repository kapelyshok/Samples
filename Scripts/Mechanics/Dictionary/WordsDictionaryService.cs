using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.Dictionary
{
    public class WordsDictionaryService : MonoBehaviour, IWordsDictionaryService
    {
        [SerializeField] private TextAsset wordsFile;

        private HashSet<string> _wordSet;
        private List<string> _wordsByLengthDesc;

        private WordsDictionary _runtimeDictionary;

        public async UniTask Initialize()
        {
            if (wordsFile == null)
            {
                Debug.LogError("Words file is not assigned!");
                return;
            }

            List<string> words = new List<string>();

            using (var reader = new StringReader(wordsFile.text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var word = line.Trim().ToLower();
                    if (!string.IsNullOrEmpty(word)) words.Add(word);
                }
            }

            _runtimeDictionary = new WordsDictionary(words);

            Debug.Log($"Loaded {_runtimeDictionary?.GetType().Name} with {words.Count} words.");
        }

        public  bool Contains(string word) => _runtimeDictionary.Contains(word);

        public async UniTask<string> GetLongestWordFromLettersAsync(IEnumerable<string> letters, int maxLettersCount)
            => await _runtimeDictionary.GetLongestWordFromLettersAsync(letters, maxLettersCount);
    }
}
