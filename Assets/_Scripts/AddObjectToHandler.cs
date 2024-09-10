using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddObjectToHandler : MonoBehaviour
{
    GameObject gameState;
    Transform Hands;

    public string interactionType;

    public bool isMachine;
    public string machineInteractionType;
    public GameObject outputObject;

    void Awake()
    {
        gameState = GameObject.Find("Game State");
        Hands = this.gameObject.transform.GetChild(1);

        Hands.gameObject.SetActive(false);

        MovableObject movableObject = new MovableObject(this.gameObject, interactionType);

        if (isMachine)
        {
            Machine machine = new Machine(this.gameObject, machineInteractionType, gameState.GetComponent<MachineHandler>(),  movableObject, outputObject);
            //machine.machineFilled = true;
            gameState.GetComponent<MachineHandler>().RegisterObject(machine);

            //movableobject.machine = machine;
            //movableObject.selfMachine = true;
            //print("machine mov added");
            //print(movableobject.machine.gameObject.name);
        }

        gameState.GetComponent<ObjectHandler>().RegisterObject(movableObject);
    }
}

