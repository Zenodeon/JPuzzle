using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

using NaughtyAttributes;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] public TextMeshProUGUI tmp;
    [Space]
    [SerializeField] private float setupMovementDuration = 0.03f;
    [SerializeField] private float movementDuration = 0.08f;
    [Space]
    [SerializeField] private AnimationCurve movementCurve;
    [Space]
    [ReadOnly] public int indexID;
    [ReadOnly] public Vector2 ID;
    [ReadOnly] public Vector2 coords;
    [ReadOnly] public Vector2 previousCoords;
    [Space]
    [SerializeField] private Mask lm;
    [SerializeField] private Mask tm;
    [SerializeField] private Mask rm;
    [SerializeField] private Mask bm;

    private PuzzleBoard board;

    private BoardData brdD;

    public Vector2 moveableDir = Vector2.zero;
    public bool masterSlider = false;

    public UnityEvent<Tile, Vector2, int> OnMoved = new UnityEvent<Tile, Vector2, int>();

    public void Setup(PuzzleBoard board, int indexID, Vector2 ID, BoardData boardData)
    {
        this.board = board;
        this.indexID = indexID;
        this.ID = ID;
        tmp.text = indexID + 1 + "";

        coords = ID;

        brdD = boardData;

        UpdateTransform();
    }

    public void UpdateTransform()
    {
        rectTransform.sizeDelta = brdD.tileSize;
        rectTransform.anchoredPosition = (coords * (brdD.tileSize + brdD.tileSpacing)) + brdD.tileOffset;
    }

    public void Move(int mode = 0)
    {
        previousCoords = coords;

        Vector2 newCoord = coords + moveableDir;
        DOTween.To(() => coords, x => coords = x, newCoord, mode == 1 ? setupMovementDuration : movementDuration)
            .SetEase(movementCurve)
            .OnUpdate(UpdateTransform)
            .OnComplete(() => OnMoved.Invoke(this, previousCoords, mode));
    }

    public void SelfMove()
    {
        board.TryMoveTile(this);   
    }

    public void ShowDirection()
    {
        lm.showMaskGraphic = false;
        tm.showMaskGraphic = false;
        rm.showMaskGraphic = false;
        bm.showMaskGraphic = false;

        if(moveableDir == Vector2.left)
            lm.showMaskGraphic = true;

        if (moveableDir == Vector2.up)
            tm.showMaskGraphic = true;

        if(moveableDir == Vector2.right)
            rm.showMaskGraphic = true;

        if (moveableDir == Vector2.down)
            bm.showMaskGraphic = true;
    }
}
