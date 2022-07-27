using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaker : MonoBehaviour
{
    public static TileMaker _instance;

    public BoardUIController boardUIController;

    public Dictionary<Vector2, Sprite> spriteTiles = new Dictionary<Vector2, Sprite>();

    private Texture2D texture;

    private Vector2 textureSize;
    private Vector2 gridSize;

    private int targetTileSize;
    private Vector2 tileOffset;

    public Vector2 progress;

    private Coroutine coroutine;

    public void Awake()
    {
        _instance = this;
    }

    public void GenerateTextureTiles(Texture2D texture, Vector2 gridSize)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        spriteTiles.Clear();

        this.texture = texture;
        this.gridSize = gridSize;

        textureSize = new Vector2(texture.width, texture.height);

        if (gridSize.x >= gridSize.y)
            targetTileSize = (int)(textureSize.x / gridSize.x);
        else
            targetTileSize = (int)(textureSize.y / gridSize.y);

        progress.y = gridSize.x * gridSize.y;
        coroutine = StartCoroutine(ExtractTileTexture());
    }

    public void OnGenerationDone()
    {
        boardUIController.UpdateBoardSettings(textureAvail: true);
    }

    IEnumerator ExtractTileTexture()
    {
        yield return null;

        progress.x = 0;
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
            {
                int tXPos = targetTileSize * x;
                int tYPos = targetTileSize * ((int)(gridSize.y - 1) - y);

                Texture2D tileTexture = new Texture2D(targetTileSize, targetTileSize, TextureFormat.ARGB32, false);

                for (int px = 0; px < targetTileSize; px++)
                    for (int py = 0; py < targetTileSize; py++)
                    {
                        Color pixel = texture.GetPixel(tXPos + px, tYPos + py);
                        tileTexture.SetPixel(px, py, pixel);
                    }

                tileTexture.Apply();
                yield return null;

                Sprite sprite = Sprite.Create(tileTexture, new Rect(0, 0, targetTileSize, targetTileSize), Vector2.one * 0.5f);

                spriteTiles.Add(new Vector2(x, -y), sprite);

                progress.x++;
            }

        OnGenerationDone();
    }
}
