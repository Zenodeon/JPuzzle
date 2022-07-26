using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveCountDisplay;

    private BoardData boardData;

    public void Setup(BoardData boardData)
    {
        this.boardData = boardData;

        boardData.OnDataUpdated.RemoveListener(UpdateUI);
        boardData.OnDataUpdated.AddListener(UpdateUI);
    }

    public void UpdateUI()
    {
        moveCountDisplay.text = "Moves : " + boardData.moves;
    }
}
