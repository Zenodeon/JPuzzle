using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Bar : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    public float startWidth;
    public float endWidth;

    public float duration;

    public void UpdatePercent(float value)
    {
        float percent = Mathf.Clamp(value, 0, 1);

        float newWidth = MathUtility.RangedMapClamp(percent, 0, 1, startWidth, endWidth);
        Vector2 newSize = new Vector2(newWidth, rectTransform.sizeDelta.y);

        rectTransform.DOSizeDelta(newSize, duration);
    }
}
