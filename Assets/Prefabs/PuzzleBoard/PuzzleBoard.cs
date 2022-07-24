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
    public int shuffleCount = 100;
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

    public int moves = 0;

    private Vector2 lastDir = Vector2.zero;

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
        RemoveLastTile();
        ShuffleTiles();
    }

    void Update()
    {

    }

    private void PlaceTiles()
    {
        int index = 0;
        for (int y = 0; y < gridSize.y; y++)
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2 coord = new Vector2(x, -y);

                Tile tile = GameObject.Instantiate(tilePrefab, tileHolderRect);
                tile.Setup(index, coord, tileSize, tileSpacing, tileSize * borderTilePercent);

                tileIDList.Add(coord, tile);
                tileCoordList.Add(coord, tile);
                tiles.Add(tile);

                index ++;
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
        for (int i = 0; i < shuffleCount; i++)
        {
            var randomDir = GetVaildRandomDir();

            Tile tile = moveableTiles[randomDir.Item1];
            Vector2 oldCoord = tile.coords;

            tile.Move(randomDir.Item2);

            tileCoordList.Remove(oldCoord);
            tileCoordList.Add(tile.coords, tile);

            SetMoveableTiles(oldCoord);
        }
    }

    private (BoardInput.InputDir, Vector2) GetVaildRandomDir()
    {
        List<BoardInput.InputDir> vaildDir = new List<BoardInput.InputDir>();
        foreach (var dir in moveableTiles.Keys)
            vaildDir.Add(dir);

        var result = GetBestDir(vaildDir);
        lastDir = result.Item2;

        return result;
    }

    private (BoardInput.InputDir, Vector2) GetBestDir(List<BoardInput.InputDir> vaildDir)
    {
        var selectedDir = vaildDir[Random.Range(0, vaildDir.Count - 1)];

        Vector2 moveDir = tileDirTable[(int)selectedDir] * -1;

        if (moveDir == lastDir * -1)
        {
            vaildDir.Remove(selectedDir);
            return GetBestDir(vaildDir);
        }

        return (selectedDir, moveDir);
    }

    private void RemoveLastTile()
    {
        Vector2 lastTileID = new Vector2(gridSize.x - 1, -(gridSize.y - 1));
        Tile lastTile = tileIDList[lastTileID];

        Vector2 emptyTileCoord = lastTile.coords;

        tiles.Remove(lastTile);
        tileIDList.Remove(lastTileID);
        tileCoordList.Remove(lastTile.coords);
        Destroy(lastTile.gameObject);

        SetMoveableTiles(emptyTileCoord);
    }

    private void SetMoveableTiles(Vector2 emptyTileCoord)
    {
        foreach(Tile tile in moveableTiles.Values)
            tile.ShowDirection(BoardInput.InputDir.None);

        moveableTiles.Clear();

        for (int i = 0; i < 4; i++)
        {
            Vector2 nearByTileCoord = emptyTileCoord + tileDirTable[i];
            if (tileCoordList.ContainsKey(nearByTileCoord))
            {
                BoardInput.InputDir inputDir = (BoardInput.InputDir)i;

                Tile moveableTile = tileCoordList[nearByTileCoord];
                moveableTiles.Add(inputDir, moveableTile);

                moveableTile.ShowDirection(inputDir);
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

            Tile tile = moveableTiles[input];
            tile.OnMoved.AddListener(OnTileMoved);
            tile.Move(moveDir);
        }
    }

    public void OnTileMoved(Tile movedTile, Vector2 previousTileCoord)
    {
        movedTile.OnMoved.RemoveListener(OnTileMoved);

        tileCoordList.Remove(previousTileCoord);
        tileCoordList.Add(movedTile.coords, movedTile);

        bool gameWon = CheckTileOrder();

        if (gameWon)
        {
            Debug.Log("Game : " + moves);
        }
        else
        {
            moves++;
            moving = false;
            SetMoveableTiles(previousTileCoord);
        }
    }

    private bool CheckTileOrder()
    {
        for (int y = 0; y < gridSize.y; y++)
            for (int x = 0; x < gridSize.x; x++)
            {
                if (gridSize == new Vector2(x + 1, y + 1))
                    break;

                Vector2 coord = new Vector2(x, -y);

                if (tileIDList[coord].coords != coord)
                    return false;
            }

        return true;
    }

    public readonly List<Vector2> tileDirTable = new List<Vector2>() 
    {
        new Vector2(-1, 0),
        new Vector2(0, 1),   
        new Vector2(1, 0),
        new Vector2(0, -1)
    };
}
