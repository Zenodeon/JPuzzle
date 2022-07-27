using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BoardUIController : MonoBehaviour
{
    [SerializeField] public ShufflingWindow shufflingWindow;
    [SerializeField] public EndWindow endWindow;
    [Space]
    [SerializeField] private TextMeshProUGUI moveCountDisplay;
    [Space]
    [SerializeField] private TextMeshProUGUI timerDisplay;
    [SerializeField] private Bar timerBar;
    [Space]
    [SerializeField] private TextMeshProUGUI borderSizeDisplay;
    [SerializeField] private UnityEngine.UI.Slider borderSizeSlider;
    [Space]
    [SerializeField] private TextMeshProUGUI gridSizeDisplay;
    [SerializeField] private TextMeshProUGUI gridXSizeDisplay;
    [SerializeField] private TextMeshProUGUI gridYSizeDisplay;
    [SerializeField] private UnityEngine.UI.Slider gridXSizeSlider;
    [SerializeField] private UnityEngine.UI.Slider gridYSizeSlider;

    private BoardData boardData;

    public Stopwatch watch = new Stopwatch();

    public void Setup(BoardData boardData)
    {
        this.boardData = boardData;

        TileMaker._instance.boardUIController = this;

        boardData.OnDataUpdated.RemoveListener(UpdateUI);
        boardData.OnDataUpdated.AddListener(UpdateUI);
    }

    private void Update()
    {
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        timerDisplay.text = watch.Elapsed.ToString(@"mm\:ss");

        int milSec = int.Parse(watch.Elapsed.ToString(@"ff"));
        timerBar.UpdatePercent(MathUtility.RangedMapClamp(milSec, 0, 100, 0, 1));
    }

    public void StartTimer()
    {
        watch.Restart();
    }

    public void StopTimer()
    {
        watch.Stop();
    }

    public void ClearTimer()
    {
        watch.Reset();
    }

    public void UpdateUI()
    {
        moveCountDisplay.text = "Moves : " + boardData.moves;
    }

    public void UpdateBorderTileDisplay(float value)
    {
        if (value != 0)
            value /= 10;

        borderSizeDisplay.text = "Border Size : " + value;
    }

    public void UpdateGridXDisplay(float value)
    {
        gridXSizeDisplay.text = "X : " + value;
        UpdateGridSizeDisplay();
    }

    public void UpdateGridYDisplay(float value)
    {
        gridYSizeDisplay.text = "Y : " + value;
        UpdateGridSizeDisplay();
    }

    public void UpdateGridSizeDisplay()
    {
        gridSizeDisplay.text = $"Tile Layout : {gridXSizeSlider.value}x{gridYSizeSlider.value}";
    }

    public void ChangeTileTexture(Texture2D texture)
    {
        if(texture != null)
        {
            TileMaker._instance.GenerateTextureTiles(texture, new Vector2(gridXSizeSlider.value, gridYSizeSlider.value));
            return;
        }

        UpdateBoardSettings();
    }

    public void UpdateBoardSettings(bool textureAvail = false)
    {
        float btp = borderSizeSlider.value;
        if (btp != 0)
            btp /= 10;

        boardData.borderTilePercent = btp;
        boardData.gridSize = new Vector2(gridXSizeSlider.value, gridYSizeSlider.value);

        float avgGridSize = boardData.gridSize.y;      
        float tSize = MathUtility.RangedMapClamp(avgGridSize, 2, 6, 250, 150);

        boardData.tileSize = tSize * Vector2.one ;

        boardData.textureAvail = textureAvail;

        boardData.OnDataUpdated.Invoke();
        boardData.puzzleBoard.ReGenerateBoard();
    }
}
