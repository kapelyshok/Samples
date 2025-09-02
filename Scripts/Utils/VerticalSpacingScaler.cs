using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class VerticalSpacingScaler : MonoBehaviour
{
    [SerializeField] private float spacingForSmallScreen = 80f;    // iPhone 6 (~1334 height)
    [SerializeField] private float spacingForLargeScreen = 220f;   // iPhone 15 Pro Max (~2778 height)
    [SerializeField] private float paddingTopForSmallScreen = 60f;  // iPhone 6 (~1334 height)
    [SerializeField] private float paddingTopForLargeScreen = 150f;   // iPhone 15 Pro Max (~2778 height)

    private VerticalLayoutGroup layoutGroup;

    private void Awake()
    {
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        ApplySpacing();
    }

    private void ApplySpacing()
    {
        float screenHeight = Screen.height;

        float minHeight = 1334f;  // iPhone 6
        float maxHeight = 2778f;  // iPhone 15 Pro Max

        float t = Mathf.InverseLerp(minHeight, maxHeight, screenHeight);
        float calculatedSpacing = Mathf.Lerp(spacingForSmallScreen, spacingForLargeScreen, t);
        float calculatedPaddingTop = Mathf.Lerp(paddingTopForSmallScreen, paddingTopForLargeScreen, t);

        layoutGroup.spacing = calculatedSpacing;
        layoutGroup.padding.top = (int)calculatedPaddingTop;
    }
}