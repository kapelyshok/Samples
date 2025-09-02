using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.LettersBag
{
    public class LettersBagView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI lettersLeftCounter;

        private Sequence _sequence;

        public void UpdateLettersLeft(int lettersLeft)
        {
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(lettersLeftCounter.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), .1f)).AppendCallback(() =>
            {
                lettersLeftCounter.text = lettersLeft.ToString();
            });
            _sequence.Append(lettersLeftCounter.transform.DOScale(Vector3.one, .7f));
        }

        public void AnimateEmptyBag()
        {
            lettersLeftCounter.text = 0.ToString();
        }
    }
}