using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSurfaceToHandler : MonoBehaviour
{
    GameObject gameState;
    
    public Vector3 localPosition;

    void Awake()
    {
        gameState = GameObject.Find("Game State");

        Surface surface = new Surface(this.gameObject, localPosition);

        gameState.GetComponent<SurfaceHandler>().RegisterObject(surface);
    }
}