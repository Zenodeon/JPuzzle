using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Slider : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup cGroup;
    [Space]
    [SerializeField] private bool open = true;
    [Space]
    [SerializeField] private float openXpoint;
    [SerializeField] private float closeXpoint;
    [Space]
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve movementCurve;

    public void ToggleState()
    {
        open = !open;
        float targetPoint = open ? openXpoint : closeXpoint;
        rectTransform.DOAnchorPosX(targetPoint, duration).SetEase(movementCurve);
    }

    public void ShowSlider(bool state)
    {
        cGroup.DOFade(state? 1 : 0, 0.3f);
    }
}
