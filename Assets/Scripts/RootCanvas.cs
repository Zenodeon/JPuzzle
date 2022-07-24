using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

public class RootCanvas : MonoBehaviour
{
    public static RootCanvas _instance;

    [SerializeField] public RectTransform rectTransform;
    
    public Vector2 canvasScale => new Vector2(rectTransform.localScale.x, rectTransform.localScale.y);

    private void OnDrawGizmos()
    {
        if(!_instance)
            _instance = this;
    }

    void Start()
    {
        _instance = this;
    }

    void Update()
    {

    }
}
