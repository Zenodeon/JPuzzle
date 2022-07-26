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
    [SerializeField][ReadOnly] private int shuffleCount = 0;
    [Space]
    [SerializeField][MinValue(2)][MaxValue(10)] public Vector2 gridSize;
    [Space]
    [SerializeField] public Vector2 tileSize;
    [SerializeField] public Vector2 tileSpacing;
    [SerializeField] public float borderTilePercent;

    private Dictionary<Vector2, Tile> tileIDList = new Dictionary<Vector2, Tile>();
    private Dictionary<Vector2, Tile> tileCoordList = new Dictionary<Vector2, Tile>();
    private Dictionary<Vector2, Tile> tileCoordListBuffer = new Dictionary<Vector2, Tile>();
    private Dictionary<Vector2, Tile> moveableTiles = new Dictionary<Vector2, Tile>();
    private Dictionary<Vector2, List<Tile>> slidableTiles = new Dictionary<Vector2, List<Tile>>();

    private List<Tile> tiles = new List<Tile>();
    private Tile lastMovedTile;
    private Vector2 emptyTileCoord;
    private bool moving = false;

    private bool settingUp;
    private int tileShuffedCount = 0;

    private bool cantMove => moving || settingUp;

    public BoardData brdD = new BoardData();

    private Vector2 gSize => brdD.gridSize;

    private Vector2 tSize => brdD.tileSize;
    private Vector2 tSpacing => brdD.tileSpacing;
    private float brdTPercent => brdD.borderTilePercent;

    private int moves { get => brdD.moves; set => brdD.moves = value; }


    #region Unity Events
    private void OnValidate()
    {
        UpdateBoardData();

        UpdateBGBoardSize();
        UpdateTiles();
    }

    private void OnDrawGizmos()
    {
        Vector2 canvasScale = Vector2.one;

        if (RootCanvas._instance)
            canvasScale = RootCanvas._instance.canvasScale;
        else
            return;

        Vector2 boardSize = (bgBoard.rectTransform.sizeDelta * 0.5f) * canvasScale;
        Vector2 tlSize = tSize * canvasScale;
        Vector2 tlSizeHalf = tlSize * 0.5f;
        Vector2 tlSpacing = tSpacing * canvasScale;
        Vector2 topLeftBoardPosition = new Vector2(rectTransform.position.x - boardSize.x + tlSizeHalf.x, rectTransform.position.y + boardSize.y - tlSizeHalf.y);

        topLeftBoardPosition = topLeftBoardPosition - ((tlSize * brdTPercent) * canvasScale * new Vector2(-1, 1));

        Gizmos.color = Color.blue;
        for (int x = 0; x < gSize.x; x++)
            for (int y = 0; y < gSize.y; y++)
            {
                Vector2 coord = new Vector2(x, -y);
                Vector2 position = coord * (tlSize + tlSpacing) + topLeftBoardPosition;
                Gizmos.DrawWireCube(position, tlSize);
            }
    }

    void Start()
    {
        UpdateBoardData();

        settingUp = true;

        shuffleCount = (int)((gSize.x * gSize.y) * (gSize.x + gSize.y));

        foreach (Vector2 dir in BoardData.dirTable)
            slidableTiles.Add(dir, new List<Tile>());

        PlaceTiles();
        RemoveLastTile();
        ShuffleTile(tileShuffedCount);
    }
    #endregion

    private void UpdateBoardData()
    {
        brdD.gridSize = gridSize;

        brdD.tileSize = tileSize;
        brdD.tileSpacing = tileSpacing;
        brdD.borderTilePercent = borderTilePercent;

        brdD.moves = moves;

        brdD.OnDataUpdated.Invoke();
    }

    [Button]
    private void UpdateTiles()
    {
        foreach (var tile in tileIDList)
            tile.Value.UpdateTransform();
    }

    private void UpdateBGBoardSize()
    {
        if (!bgBoard)
            return;

        RectTransform bgbRect = bgBoard.rectTransform;
        bgbRect.sizeDelta = (tSize * gSize) + (tSpacing * (gSize - Vector2.one)) + (tSize * brdTPercent * 2);
    }

    private void PlaceTiles()
    {
        int index = 0;
        for (int y = 0; y < gSize.y; y++)
            for (int x = 0; x < gSize.x; x++)
            {
                Vector2 coord = new Vector2(x, -y);

                Tile tile = GameObject.Instantiate(tilePrefab, tileHolderRect);
                tile.Setup(this, index, coord, brdD);

                tileIDList.Add(coord, tile);
                tileCoordList.Add(coord, tile);
                tiles.Add(tile);

                index++;
            }
    }

    private void ShuffleTile(int index)
    {
        if (index > shuffleCount)
        {
            settingUp = false;
            SetMoveableTiles(emptyTileCoord);
            return;
        }

        Tile randomTile = GetRandomTile();

        randomTile.OnMoved.AddListener(OnTileMoved);
        randomTile.Move(mode: 1);
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
        Vector2 lastTileID = new Vector2(gSize.x - 1, -(gSize.y - 1));
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
        this.emptyTileCoord = emptyTileCoord;
        ClearMoveableTiles();

        for (int i = 0; i < 4; i++)
        {
            Vector2 lookingDir = BoardData.dirTable[i];

            Vector2 nearByTileCoord = emptyTileCoord + lookingDir;
            if (tileCoordList.ContainsKey(nearByTileCoord))
            {
                Tile moveableTile = tileCoordList[nearByTileCoord];
                moveableTile.moveableDir = lookingDir * -1;
                moveableTile.ShowDirection();

                moveableTiles.Add(moveableTile.moveableDir, moveableTile);

                if (!settingUp)
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
        if (cantMove)
            return;

        Vector2 movingDir = BoardData.dirTable[(int)input] * -1;
        if (moveableTiles.ContainsKey(movingDir))
            MoveTile(moveableTiles[movingDir]);
    }

    public void TryMoveTile(Tile tile)
    {
        if (cantMove)
            return;

        if (!moveableTiles.ContainsKey(tile.moveableDir))
            return;

        List<Tile> tiles = slidableTiles[tile.moveableDir * -1];
        if (tiles.Contains(tile))
        {
            tile.masterSlider = true;
            int tileIndex = tiles.IndexOf(tile);

            moving = true;

            int currentIndex = tileIndex++;
            while (currentIndex >= 0)
                SlideTile(tiles[currentIndex--]);

            tile = moveableTiles[tile.moveableDir];
        }

        MoveTile(tile);
    }

    public void SlideTile(Tile tile)
    {
        tile.OnMoved.AddListener(OnTileMoved);
        tile.Move(mode: 2);
    }

    public void MoveTile(Tile tile)
    {
        moving = true;

        tile.OnMoved.AddListener(OnTileMoved);
        tile.Move();
    }

    public void OnTileMoved(Tile movedTile, Vector2 previousTileCoord, int mode)
    {
        movedTile.OnMoved.RemoveListener(OnTileMoved);

        tileCoordList.Remove(previousTileCoord);

        if (mode == 2)
        {
            tileCoordListBuffer.Add(movedTile.coords, movedTile);
            return;
        }

        if (mode == 1)
        {
            tileCoordList.Add(movedTile.coords, movedTile);

            SetMoveableTiles(previousTileCoord);
            ShuffleTile(tileShuffedCount++);
            return;
        }

        if (mode == 0)
        {
            Vector2 emptyCoord = Vector2.one * -1;

            foreach (var bufferTile in tileCoordListBuffer)
            {
                Tile tile = bufferTile.Value;

                if (emptyCoord == (Vector2.one * -1) && tile.masterSlider)
                {
                    tile.masterSlider = false;
                    emptyCoord = tile.previousCoords;
                }

                tileCoordList.Add(bufferTile.Key, tile);
            }
            tileCoordListBuffer.Clear();

            tileCoordList.Add(movedTile.coords, movedTile);

            bool gameWon = CheckTileOrder();

            if (gameWon)
                GameWon();
            else
            {
                if (emptyCoord == Vector2.one * -1)
                    emptyCoord = previousTileCoord;

                moves++;
                moving = false;
                SetMoveableTiles(emptyCoord);
            }
            return;
        }
    }

    private bool CheckTileOrder()
    {
        for (int y = 0; y < gSize.y; y++)
            for (int x = 0; x < gSize.x; x++)
            {
                if (gSize == new Vector2(x + 1, y + 1))
                    break;

                Vector2 coord = new Vector2(x, -y);

                if (tileIDList[coord].coords != coord)
                    return false;
            }

        return true;
    }

    private void GameWon()
    {
        ClearMoveableTiles();
        Debug.Log("Game : " + moves);
    }
}
