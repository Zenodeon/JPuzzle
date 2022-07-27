using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BoardUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveCountDisplay;
    [Space]
    [SerializeField] private TextMeshProUGUI timerDisplay;
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

    public void UpdateBoardSettings()
    {
        float btp = borderSizeSlider.value;
        if (btp != 0)
            btp /= 10;

        boardData.borderTilePercent = btp;
        boardData.gridSize = new Vector2(gridXSizeSlider.value, gridYSizeSlider.value);

        float avgGridSize = boardData.gridSize.y;      
        float tSize = RangedMapClamp(avgGridSize, 2, 6, 250, 150);

        boardData.tileSize = tSize * Vector2.one ;

        boardData.OnDataUpdated.Invoke();
        boardData.puzzleBoard.ReGenerateBoard();

    }

    public static float RangedMapClamp(float value, float InMinimum, float InMaximum, float OutMinimum, float OutMaximum)
    {
        var InRange = InMaximum - InMinimum;
        var OutRange = OutMaximum - OutMinimum;
        var finalValue = ((value - InMinimum) * OutRange / InRange) + OutMinimum;

        return finalValue;
    }
}
