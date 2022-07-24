using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using NaughtyAttributes;

public class Tile : MonoBehaviour
{
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] public TextMeshProUGUI tmp;

    [ReadOnly] public Vector2 ID;
    [ReadOnly] public Vector2 coords;

    private Vector2 size;
    private Vector2 spacing;
    private Vector2 offset;

    public BoardInput.InputDir moveableDirection = BoardInput.InputDir.None;

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
        Debug.Log(ID + " : " + dir + " : " + moveableDirection.ToString());
        coords += dir;
        UpdateTransform();
    }
}
