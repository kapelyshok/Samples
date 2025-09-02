using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float pressedScale = 0.9f;
    [SerializeField] private float animationDuration = 0.1f;

    private RectTransform rectTransform;
    private Tween currentTween;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AnimateTo(pressedScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AnimateTo(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateTo(1f);
    }

    private void AnimateTo(float scale)
    {
        currentTween?.Kill();
        currentTween = rectTransform.DOScale(scale, animationDuration).SetEase(Ease.OutQuad);
    }
}