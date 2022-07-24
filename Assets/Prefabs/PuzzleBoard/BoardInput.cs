using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoardInput : MonoBehaviour
{
    public UnityEvent<InputDir> inputEvent = new UnityEvent<InputDir>();

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) | Input.GetKeyDown(KeyCode.UpArrow))
            inputEvent.Invoke(InputDir.Up);

        if (Input.GetKeyDown(KeyCode.A) | Input.GetKeyDown(KeyCode.LeftArrow))
            inputEvent.Invoke(InputDir.Left);


        if (Input.GetKeyDown(KeyCode.S) | Input.GetKeyDown(KeyCode.DownArrow))
            inputEvent.Invoke(InputDir.Down);


        if (Input.GetKeyDown(KeyCode.D) | Input.GetKeyDown(KeyCode.RightArrow))
            inputEvent.Invoke(InputDir.Right);
    }

    public enum InputDir
    {
        Right,
        Down,
        Left,
        Up,
        None
    }
}
