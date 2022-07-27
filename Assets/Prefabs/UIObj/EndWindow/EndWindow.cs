using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class EndWindow : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [Space]
    [SerializeField] private Vector2 startScale;
    [SerializeField] private Vector2 endScale;
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve startMovementCurve;
    [SerializeField] private AnimationCurve endMovementCurve;

    public void ShowWindow(float delay)
    {
        rectTransform.DOScale(endScale, duration).SetEase(endMovementCurve).SetDelay(delay).OnComplete(() => HideWindow(1));
    }

    public void HideWindow(float delay)
    {
        rectTransform.DOScale(startScale, duration).SetEase(startMovementCurve).SetDelay(delay);
    }
}
