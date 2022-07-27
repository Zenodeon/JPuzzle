using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaker : MonoBehaviour
{
    public static TileMaker _instance;

    Dictionary<Vector2, Texture2D> textureTiles = new Dictionary<Vector2, Texture2D>();

    private Texture2D texture;

    private Vector2 textureSize;
    private Vector2 gridSize;

    private int targetTileSize;
    private Vector2 tileOffset;

    private Vector2 progress;

    public void Awake()
    {
        _instance = this;
    }

    public void GenerateTextureTiles(Texture2D texture, Vector2 gridSize)
    {
        this.texture = texture;
        this.gridSize = gridSize;

        textureSize = new Vector2(texture.width, texture.height);

        if (gridSize.x >= gridSize.y)
            targetTileSize = (int)(textureSize.x / gridSize.x);
        else
            targetTileSize = (int)(textureSize.y / gridSize.y);

        progress.y = gridSize.x * gridSize.y;
        StartCoroutine(ExtractTileTexture());
    }

    public void OnGenerationDone()
    {

    }

    IEnumerator ExtractTileTexture()
    {
        yield return null;

        progress.x = 0;
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
            {
                int tXPos = targetTileSize * x;
                int tYPos = targetTileSize * y;

                Texture2D tileTexture = new Texture2D(targetTileSize, targetTileSize);

                for (int px = 0; px < targetTileSize; px++)
                    for (int py = 0; py < targetTileSize; py++)
                    {
                        Color pixel = texture.GetPixel(tXPos + px, tYPos + py);
                        tileTexture.SetPixel(px, py, pixel);
                    }

                textureTiles.Add(new Vector2(x, -y), tileTexture);

                progress.x++;
                Debug.Log(progress.x + " : Done");
            }

        OnGenerationDone();
    }
}
