using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StyleButton : MonoBehaviour
{
    [SerializeField] private Texture2D texture;

    public UnityEvent<Texture2D> OnStyleChange = new UnityEvent<Texture2D>();

    public void ChangeStyle()
    {
        OnStyleChange.Invoke(texture);
    }
}
