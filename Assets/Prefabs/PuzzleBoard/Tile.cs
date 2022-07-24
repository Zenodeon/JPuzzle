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
    [SerializeField] public AnimationCurve moveCurve;
    [Space]
    [ReadOnly] public int indexID;
    [ReadOnly] public Vector2 ID;
    [ReadOnly] public Vector2 coords;
    [ReadOnly] public Vector2 previousCoords;

    private Vector2 size;
    private Vector2 spacing;
    private Vector2 offset;

    public UnityEvent<Tile, Vector2, bool> OnMoved = new UnityEvent<Tile, Vector2, bool>();

    public void Setup(int indexID, Vector2 ID, Vector2 size, Vector2 spacing, Vector2 offset)
    {
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

    public void Move(Vector2 dir, bool setupMode = false)
    {
        previousCoords = coords;

        Vector2 newCoord = coords + dir;
        DOTween.To(() => coords, x => coords = x, newCoord, setupMode ? 0.05f : 2)
            .SetEase(moveCurve)
            .OnUpdate(UpdateTransform)
            .OnComplete(() => OnMoved.Invoke(this, previousCoords, setupMode));
    }

    public void ShowDirection(BoardInput.InputDir dir)
    {
        switch (dir)
        {
            case BoardInput.InputDir.Up:
                break;

            case BoardInput.InputDir.Down:
                break;

            case BoardInput.InputDir.Left:
                break;

            case BoardInput.InputDir.Right:
                break;

            default: break;
        }
    }
}
