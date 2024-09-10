using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDispenserToHandler : MonoBehaviour
{
    GameObject gameState;

    public GameObject outputObject;

    // Start is called before the first frame update
    void Awake()
    {
        gameState = GameObject.Find("Game State");

        gameState.GetComponent<DispenserHandler>().RegisterDispenser(this.gameObject, outputObject);
    }

}
