using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StyleButton : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Texture2D texture;

    public UnityEvent<Sprite, Texture2D> OnStyleChange = new UnityEvent<Sprite, Texture2D>();

    public void ChangeStyle()
    {
        OnStyleChange.Invoke(sprite, texture);
    }
}
