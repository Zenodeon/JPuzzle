using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoardData
{
    public UnityEvent OnDataUpdated = new UnityEvent();

    public Vector2 gridSize;

    public Vector2 tileSize;
    public Vector2 tileSpacing;
    public float borderTilePercent;
    public Vector2 tileOffset => (tileSize * borderTilePercent) * new Vector2(1, -1);

    private int _moves;
    public int moves
    {
        get => _moves;
        set
        {
            _moves = value;
            OnDataUpdated.Invoke();
        }
    }

    public static readonly List<Vector2> dirTable = new List<Vector2>()
    {
        Vector2.left,
        Vector2.up,
        Vector2.right,
        Vector2.down
    };
}
