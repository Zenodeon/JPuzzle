using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

using NaughtyAttributes;

public class Tile : MonoBehaviour
{
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] public TextMeshProUGUI tmp;
    [Space]
    [ReadOnly] public Vector2 ID;
    [ReadOnly] public Vector2 coords;
    [ReadOnly] public Vector2 previousCoords;

    private Vector2 size;
    private Vector2 spacing;
    private Vector2 offset;

    public UnityEvent<Tile, Vector2> OnMoved = new UnityEvent<Tile, Vector2>();

    public void Setup(Vector2 ID, Vector2 size, Vector2 spacing, Vector2 offset)
    {
        this.ID = ID;
        tmp.text = ID.x + "," + ID.y;
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

    public void Move(Vector2 dir)
    {
        previousCoords = coords;
        coords += dir;

        UpdateTransform();

        OnMoved.Invoke(this, previousCoords);
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
