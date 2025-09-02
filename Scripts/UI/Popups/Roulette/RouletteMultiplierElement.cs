using UnityEngine;

namespace Unavinar
{
    [RequireComponent(typeof(RectTransform))]
    public class RouletteMultiplierElement : MonoBehaviour
    {
        [field: SerializeField, Min(0f)] public float MultiplierValue { get; private set; }
        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }
    }
}
