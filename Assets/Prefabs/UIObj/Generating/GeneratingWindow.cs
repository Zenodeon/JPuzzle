using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class GeneratingWindow : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup cGroup;
    [Space]
    [SerializeField] private Bar bar;
    [Space]
    [SerializeField] private float startYPos;
    [SerializeField] private float endYPos;
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve startMovementCurve;
    [SerializeField] private AnimationCurve endMovementCurve;

    bool active;

    public void ShowWindow()
    {
        active = true;
        rectTransform.DOAnchorPosY(endYPos, duration).SetEase(endMovementCurve);
        cGroup.DOFade(1, duration);
    }

    public void HideWindow()
    {
        active = false;
        rectTransform.DOAnchorPosY(startYPos, duration).SetEase(startMovementCurve);
        cGroup.DOFade(0, duration);
    }

    private void Update()
    {
        if (!active)
            return;

        TileMaker tm = TileMaker._instance;
        bar.UpdatePercent(MathUtility.RangedMapClamp(tm.progress.x, 0, tm.progress.y, 0, 1));
    }
}
