using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;

public class PuzzleBoard : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image bgBoard;
    [SerializeField] private RectTransform tileHolderRect;
    [SerializeField] private Tile tilePrefab;
    [Space]
    [SerializeField][MinValue(2)][MaxValue(10)] public Vector2 gridSize;
    [Space]
    [SerializeField] public Vector2 tileSize;
    [SerializeField] public Vector2 tileSpacing;
    [SerializeField] public float borderTilePercent;

    public Dictionary<Vector2, Tile> tileIDList = new Dictionary<Vector2, Tile>();
    public Dictionary<Vector2, Tile> tileCoordList = new Dictionary<Vector2, Tile>();
    public List<Tile> tiles = new List<Tile>();
    public Dictionary<BoardInput.InputDir, Tile> moveableTiles = new Dictionary<BoardInput.InputDir, Tile>();

    public bool moving = false;

    private void OnValidate()
    {
        UpdateBGBoardSize();

        UpdateTiles();
    }

    private void UpdateBGBoardSize()
    {
        if (!bgBoard)
            return;

        RectTransform bgbRect = bgBoard.rectTransform;
        bgbRect.sizeDelta = (tileSize * gridSize) + (tileSpacing * (gridSize - Vector2.one)) + (tileSize * borderTilePercent * 2);
    }

    private void OnDrawGizmos()
    {
        Vector2 canvasScale = Vector2.one;

        if (RootCanvas._instance)
            canvasScale = RootCanvas._instance.canvasScale;

        Vector2 boardSize = (bgBoard.rectTransform.sizeDelta * 0.5f) * canvasScale;
        Vector2 tSize = tileSize * canvasScale;
        Vector2 tSizeHalf = tSize * 0.5f;
        Vector2 tSpacing = tileSpacing * canvasScale;
        Vector2 topLeftBoardPosition = new Vector2(rectTransform.position.x - boardSize.x + tSizeHalf.x, rectTransform.position.y + boardSize.y - tSizeHalf.y);

        topLeftBoardPosition = topLeftBoardPosition - ((tSize * borderTilePercent) * canvasScale * new Vector2(-1, 1));

        Gizmos.color = Color.blue;
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2 coord = new Vector2(x, -y);
                Vector2 position = coord * (tSize + tSpacing) + topLeftBoardPosition;
                Gizmos.DrawWireCube(position, tSize);
            }
    }

    void Start()
    {
        PlaceTiles();
        ShuffleTiles();
        RemoveLastTile();
    }

    void Update()
    {

    }

    private void PlaceTiles()
    {
        for (int y = 0; y < gridSize.y; y++)
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2 coord = new Vector2(x, -y);

                Tile tile = GameObject.Instantiate(tilePrefab, tileHolderRect);
                tile.Setup(coord, tileSize, tileSpacing, tileSize * borderTilePercent);

                tileIDList.Add(coord, tile);
                tiles.Add(tile);
            }
    }

    [Button]
    private void UpdateTiles()
    {
        foreach (var tile in tileIDList)
        {
            tile.Value.UpdateTransform(tileSize, tileSpacing, tileSize * borderTilePercent);
        }
    }

    private void ShuffleTiles()
    {
        List<Tile> tilesToShuffle = new List<Tile>(tiles);

        for (int y = 0; y < gridSize.y; y++)
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2 coords = new Vector2(x, -y);

                int randomTileIndex = Random.Range(0, tilesToShuffle.Count);

                Tile randomTile = tilesToShuffle[randomTileIndex];
                randomTile.coords = coords;
                randomTile.UpdateTransform();

                tileCoordList.Add(coords, randomTile);
                tilesToShuffle.Remove(randomTile);
            }
    }

    private void RemoveLastTile()
    {
        Vector2 lastTileID = new Vector2(gridSize.x - 1, -(gridSize.y - 1));
        Tile lastTile = tileIDList[lastTileID];

        Vector2 emptyTileCoord = lastTile.coords;

        tiles.Remove(lastTile);
        tileIDList.Remove(lastTileID);
        Destroy(lastTile.gameObject);

        SetMoveableTiles(emptyTileCoord);
    }

    private void SetMoveableTiles(Vector2 emptyTileCoord)
    {
        moveableTiles.Clear();
        for (int i = 0; i < 4; i++)
        {
            Vector2 nearByTileCoord = emptyTileCoord + tileDirTable[i];
            if (tileCoordList.ContainsKey(nearByTileCoord))
            {
                BoardInput.InputDir inputDir = (BoardInput.InputDir)i;

                Tile moveableTile = tileCoordList[nearByTileCoord];
                moveableTiles.Add(inputDir, moveableTile);
            }
        }
    }

    public void OnKeyInput(BoardInput.InputDir input)
    {
        if (moving)
            return;

        if (moveableTiles.ContainsKey(input))
        {
            moving = true;
            Vector2 moveDir = tileDirTable[(int)input] * -1;
            moveableTiles[input].Move(moveDir);
        }
    }

    public void OnTileMoved(Vector2 previousTileCoord)
    {
        SetMoveableTiles(previousTileCoord);
        moving = false;
    }

    public readonly List<Vector2> tileDirTable = new List<Vector2>() 
    {
        new Vector2(-1, 0),
        new Vector2(0, 1),   
        new Vector2(1, 0),
        new Vector2(0, -1)
    };
}
