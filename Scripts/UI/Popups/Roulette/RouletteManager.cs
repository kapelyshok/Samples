using System.Collections.Generic;
using UnityEngine;

namespace Unavinar
{
    public class RouletteManager : MonoBehaviour
    {
        [SerializeField] private RectTransform arrow;
        [SerializeField] private List<RouletteMultiplierElement> multipliers;
        [SerializeField] private float movementSpeed = 200f;

        private bool _isMovingRight = true;
        private bool _isRouletteActive;
        
        public bool IsRouletteActive => _isRouletteActive;

        private void Update()
        {
            if (!_isRouletteActive) return;

            var delta = movementSpeed * Time.deltaTime;
            if (_isMovingRight)
            {
                arrow.anchoredPosition += new Vector2(delta, 0);

                if (arrow.anchoredPosition.x >= GetRightEdge()) _isMovingRight = false;
            }
            else
            {
                arrow.anchoredPosition -= new Vector2(delta, 0);

                if (arrow.anchoredPosition.x <= GetLeftEdge()) _isMovingRight = true;
            }
        }

        public void Run()
        {
            _isRouletteActive = true;
        }

        public void Stop()
        {
            _isRouletteActive = false;
        }

        public float GetMultiplier()
        {
            var closestMultiplier = GetClosestMultiplier();

            if (closestMultiplier != null)
            {
               // Debug.Log($"Roulette multiplier: {closestMultiplier.MultiplierValue}", gameObject);
                return closestMultiplier.MultiplierValue;
            }
            else
            {
                Debug.LogError("Smth went wrong with calculating nearest roulette multiplier", gameObject);
                return 1f;
            }
        }

        private RouletteMultiplierElement GetClosestMultiplier()
        {
            RouletteMultiplierElement closestMultiplier = null;
            var closestDistance = float.MaxValue;

            var arrowWorldPosition = arrow.position;

            foreach (var multiplier in multipliers)
            {
                var multiplierCenter = multiplier.RectTransform.position;

                var distance = Mathf.Abs(arrowWorldPosition.x - multiplierCenter.x);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestMultiplier = multiplier;
                }
            }

            return closestMultiplier;
        }

        private float GetLeftEdge()
        {
            return arrow.parent.InverseTransformPoint(multipliers[0].RectTransform.position).x -
                   multipliers[0].RectTransform.rect.width / 2;
        }

        private float GetRightEdge()
        {
            return arrow.parent.InverseTransformPoint(multipliers[^1].RectTransform.position).x +
                   multipliers[^1].RectTransform.rect.width / 2;
        }
    }
}
