using System.Collections.Generic;
using AtomicApps.Infrastructure.Services.Audio;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.LettersBag
{
    public class LettersBagManager : MonoBehaviour
    {
        [SerializeField]
        private LettersBagView lettersBagView;

        [SerializeField]
        private LettersBagData lettersBagData;

        [SerializeField]
        private ParticleSystem lightImpact;

        private LinkedList<TileEntry> _availableLetters = new LinkedList<TileEntry>();
        private bool _isCounterLocked = false;
        private IAudioService _audioService;
        private LetterEntry _nextRandomLetterEntry;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }
        
        public void InitializeLettersBag()
        {
            RefillAvailableLetters();
            UpdateLettersBagInfo();
        }

        public LinkedList<TileEntry> GetAvailableLetters()
        {
            return _availableLetters;
        }

        public void RemoveTileEntryFromBag(TileEntry tileEntry)
        {
            var node = _availableLetters.First;
            while (node != null)
            {
                var next = node.Next;
                if (node.Value.Equals(tileEntry))
                {
                    _availableLetters.Remove(node);
                    break;
                }
                node = next;
            }

            UpdateLettersBagInfo();
        }

        public void RemoveTileEntryFromBagAndRefresh(TileEntry tileEntry)
        {
            RemoveTileEntryFromBag(tileEntry);
            ShuffleAvailableList();
        }

        public void AddTileEntryToBagAndRefresh(TileEntry tileEntry)
        {
            _audioService.PlaySound(SoundKeys.BAG_LETTERS_ADDED);
            AddTileEntryToBag(tileEntry);
            ShuffleAvailableList();
        }

        public void AddTileEntryToBag(TileEntry tileEntry)
        {
            _availableLetters.AddLast(tileEntry);
            UpdateLettersBagInfo();
        }

        public void ShuffleAvailableList()
        {
            List<TileEntry> tempList = new List<TileEntry>(_availableLetters);

            System.Random rng = new System.Random();
            int n = tempList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (tempList[k], tempList[n]) = (tempList[n], tempList[k]);
            }

            _availableLetters.Clear();
            foreach (var item in tempList)
            {
                _availableLetters.AddLast(item);
            }
        }

        public void ChangeCounterLockedState(bool state)
        {
            _isCounterLocked = state;
        }

        private void NotifyEmptyBag()
        {
            lettersBagView.AnimateEmptyBag();
        }
        
        public void UpdateLettersBagInfo()
        {
            if (!_isCounterLocked)
            {
                lettersBagView.UpdateLettersLeft(_availableLetters.Count);
            }
        }

        private void RefillAvailableLetters()
        {
            List<TileEntry> letterList = new List<TileEntry>();

            foreach (var letterEntry in lettersBagData.Letters)
            {
                for (int i = 0; i < letterEntry.Amount; i++)
                {
                    letterList.Add(new TileEntry()
                    {
                        LetterEntry = new LetterEntry()
                        {
                            Amount = letterEntry.Amount,
                            Letter = letterEntry.Letter,
                            Points = letterEntry.Points
                        },
                        Tile = Tile.DEFAULT
                    });
                }
            }

            System.Random rng = new System.Random();
            int n = letterList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (letterList[k], letterList[n]) = (letterList[n], letterList[k]);
            }

            _availableLetters = new LinkedList<TileEntry>(letterList);
        }

        public TileEntry GetNextRandomLetterFromBag()
        {
            if (_availableLetters.Count > 0)
            {
                var letterEntry = _availableLetters.First.Value;
                _availableLetters.RemoveFirst();
                UpdateLettersBagInfo();
                return letterEntry;
            }
            else
            {
                NotifyEmptyBag();
                return null;
            }
        }

        public LetterEntry GetRandomLetterEntry()
        {
            if (_nextRandomLetterEntry != null)
            {
                var newLetterEntry = new LetterEntry()
                {
                    Amount = _nextRandomLetterEntry.Amount,
                    Letter = _nextRandomLetterEntry.Letter,
                    Points = _nextRandomLetterEntry.Points
                };
                _nextRandomLetterEntry = null;
                return newLetterEntry;
            }
            
            return lettersBagData.Letters[Random.Range(0, lettersBagData.Letters.Count)];
        }
        
        public LetterEntry PeekNextRandomLetterEntry()
        {
            _nextRandomLetterEntry = lettersBagData.Letters[Random.Range(0, lettersBagData.Letters.Count)];
            
            var newLetterEntry = new LetterEntry()
            {
                Amount = _nextRandomLetterEntry.Amount,
                Letter = _nextRandomLetterEntry.Letter,
                Points = _nextRandomLetterEntry.Points
            };
            _nextRandomLetterEntry = newLetterEntry;
            
            return _nextRandomLetterEntry;
        }

        public void PlayLightningImpact()
        {
            lightImpact.Play();
        }

        public LettersBagView GetLettersBagView()
        {
            return lettersBagView;
        }
    }
}
