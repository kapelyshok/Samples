using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace AtomicApps.Mechanics.Gameplay.Dictionary
{
    public interface IWordsDictionaryService
    {
        public UniTask Initialize();
        public UniTask<string> GetLongestWordFromLettersAsync(IEnumerable<string> letters, int maxLettersCount);
        public bool Contains(string word);
    }
}