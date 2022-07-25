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
    [SerializeField] private AnimationCurve tileMovementCurve;
    [Space]
    [SerializeField] [ReadOnly] private int shuffleCount = 0;
    [Space]
    [SerializeField][MinValue(2)][MaxValue(10)] public Vector2 gridSize;
    [Space]
    [SerializeField] public Vector2 tileSize;
    [SerializeField] public Vector2 tileSpacing;
    [SerializeField] public float borderTilePercent;

    public Dictionary<Vector2, Tile> tileIDList = new Dictionary<Vector2, Tile>();
    public Dictionary<Vector2, Tile> tileCoordList = new Dictionary<Vector2, Tile>();
    public Dictionary<Vector2, Tile> moveableTiles = new Dictionary<Vector2, Tile>();
    public Dictionary<Vector2, List<Tile>> slidableTiles = new Dictionary<Vector2, List<Tile>>();

    public List<Tile> tiles = new List<Tile>();

    public int moves = 0;

    private bool settingUp;
    private int tileShuffedCount = 0;

    private bool moving = false;

    private Tile lastMovedTile;

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
        settingUp = true;

        shuffleCount = (int)((gridSize.x * gridSize.y) * (gridSize.x + gridSize.y));

        foreach (Vector2 dir in tileDirTable)
            slidableTiles.Add(dir, new List<Tile>());

        PlaceTiles();
        RemoveLastTile();
        ShuffleTile(tileShuffedCount);
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
                tile.Setup(this, index, coord, tileSize, tileSpacing, tileSize * borderTilePercent);

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
            tile.Value.UpdateTransform(tileSize, tileSpacing, tileSize * borderTilePercent);
    }

    private void ShuffleTile(int index)
    {
        if (index > shuffleCount)
        {
            settingUp = false;
            return;
        }

        Tile randomTile = GetRandomTile();

        randomTile.OnMoved.AddListener(OnTileMoved);
        randomTile.Move(tileMovementCurve, setupMode: true);
    }

    private Tile GetRandomTile(List<Tile> availTiles = null)
    {
        if (availTiles == null)
            availTiles = new List<Tile>(moveableTiles.Values);

        int availTileCount = availTiles.Count;
        int randomIndex = Random.Range(0, availTileCount);

        Tile randomTile = availTiles[randomIndex];
        if (lastMovedTile)
            if (randomTile.ID == lastMovedTile.ID)
            {
                availTiles.Remove(randomTile);
                return GetRandomTile(availTiles);
            }

        lastMovedTile = randomTile;
        return randomTile;
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
        ClearMoveableTiles();

        for (int i = 0; i < 4; i++)
        {
            Vector2 lookingDir = tileDirTable[i];

            Vector2 nearByTileCoord = emptyTileCoord + lookingDir;
            if (tileCoordList.ContainsKey(nearByTileCoord))
            {
                Tile moveableTile = tileCoordList[nearByTileCoord];
                moveableTile.moveableDir = lookingDir * -1;
                moveableTile.ShowDirection();

                moveableTiles.Add(moveableTile.moveableDir, moveableTile);

                slidableTiles[lookingDir] = GetTilesInDir(nearByTileCoord, lookingDir);
            }
        }
    }

    private List<Tile> GetTilesInDir(Vector2 startCoord, Vector2 dir)
    {
        Vector2 currentCoord = startCoord;
        bool tileExistInDir() => tileCoordList.ContainsKey(currentCoord + dir);

        List<Tile> slidableTiles = new List<Tile>();
        while (tileExistInDir())
        {
            Tile slidableTile = tileCoordList[currentCoord += dir];

            slidableTile.moveableDir = dir * -1;
            slidableTile.ShowDirection();

            slidableTiles.Add(slidableTile);
        }
        return slidableTiles;
    }

    private void ClearMoveableTiles()
    {
        foreach (Tile tile in moveableTiles.Values)
        {
            tile.moveableDir = Vector2.zero;
            tile.ShowDirection();
        }
        moveableTiles.Clear();

        foreach (List<Tile> tileList in slidableTiles.Values)
        {
            foreach (Tile tile in tileList)
            {
                tile.moveableDir = Vector2.zero;
                tile.ShowDirection();
            }
            tileList.Clear();
        }
    }

    public void OnKeyInput(BoardInput.InputDir input)
    {
        if (moving)
            return;

        Vector2 movingDir = tileDirTable[(int)input] * -1;
        if (moveableTiles.ContainsKey(movingDir))
            MoveTile(moveableTiles[movingDir]);
    }

    public void OnTileMoved(Tile movedTile, Vector2 previousTileCoord, bool setupMode)
    {
        movedTile.OnMoved.RemoveListener(OnTileMoved);

        tileCoordList.Remove(previousTileCoord);
        tileCoordList.Add(movedTile.coords, movedTile);

        if (setupMode)
        {
            SetMoveableTiles(previousTileCoord);
            ShuffleTile(tileShuffedCount++);
        }
        else
        {
            bool gameWon = CheckTileOrder();

            if (gameWon)
            {
                ClearMoveableTiles();
                Debug.Log("Game : " + moves);
            }
            else
            {
                moves++;
                moving = false;
                SetMoveableTiles(previousTileCoord);
            }
        }
    }

    public void TryMoveTile(Tile tile)
    {
        if (moving)
            return;

        if (moveableTiles.ContainsKey(tile.moveableDir))
            MoveTile(tile);
    }

    public void MoveTile(Tile tile)
    {
        moving = true;

        tile.OnMoved.AddListener(OnTileMoved);
        tile.Move(tileMovementCurve);
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
        Vector2.left,
        Vector2.up,   
        Vector2.right,
        Vector2.down
    };
}
