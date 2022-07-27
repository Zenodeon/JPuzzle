using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class ShufflingWindow : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [Space]
    [SerializeField] private Bar bar;
    [Space]
    [SerializeField] private float startYPos;
    [SerializeField] private float endYPos;
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve startMovementCurve;
    [SerializeField] private AnimationCurve endMovementCurve;
    private BoardData boardData;

    public void ShowWindow(BoardData boardData)
    {
        this.boardData = boardData;
        boardData.OnDataUpdated.AddListener(DataChanged);

        rectTransform.DOAnchorPosY(endYPos, duration).SetEase(endMovementCurve); ;
    }

    public void HideWindow()
    {
        rectTransform.DOAnchorPosY(startYPos, duration).SetEase(startMovementCurve);
    }

    private void DataChanged()
    {
        bar.UpdatePercent(MathUtility.RangedMapClamp(boardData.currentShuffleCount, 0, boardData.totalShuffleCount, 0, 1));
    }
}
