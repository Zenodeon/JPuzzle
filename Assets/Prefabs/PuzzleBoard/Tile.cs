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

    private Vector2 size;
    private Vector2 spacing;
    private Vector2 offset;

    public Vector2 moveableDir = Vector2.zero;

    public UnityEvent<Tile, Vector2, bool> OnMoved = new UnityEvent<Tile, Vector2, bool>();

    public void Setup(PuzzleBoard board, int indexID, Vector2 ID, Vector2 size, Vector2 spacing, Vector2 offset)
    {
        this.board = board;
        this.indexID = indexID;
        this.ID = ID;
        tmp.text = indexID + "";
        //tmp.text = ID.x + "," + ID.y;
        coords = ID;

        UpdateTransform(size, spacing, offset);
    }

    public void UpdateTransform(Vector2 size, Vector2 spacing, Vector2 offset)
    {
        this.size = size;
        this.spacing = spacing * new Vector2(1, 1);
        this.offset = offset * new Vector2(1, -1);

        UpdateTransform();
    }

    public void UpdateTransform()
    {
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = (coords * (size + spacing)) + offset;
    }

    public void Move(AnimationCurve movementCurve, bool setupMode = false)
    {
        previousCoords = coords;

        Vector2 newCoord = coords + moveableDir;
        DOTween.To(() => coords, x => coords = x, newCoord, setupMode ? 0.04f : 0.1f)
            .SetEase(movementCurve)
            .OnUpdate(UpdateTransform)
            .OnComplete(() => OnMoved.Invoke(this, previousCoords, setupMode));
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
