using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AddMachineToHandler : MonoBehaviour
{
    GameObject gameState;

    public Vector3 placementPosition;
    public string interactionType;
    public int interactionTime;

    public bool hasOutput = false;
    public GameObject outputObject = null;

    public bool hasAnimation = false;
    public int animationLayer;
    Animator animator;



    public bool destroyMachineOnCompletion;


    void Awake()
    {
        gameState = GameObject.Find("Game State");
        

        Machine machine;

        if (hasOutput)
        {
            machine = new Machine(this.gameObject, interactionType, placementPosition, interactionTime,  gameState.GetComponent<MachineHandler>(), outputObject, destroyMachineOnCompletion);
        }
        else
        {
            machine = new Machine(this.gameObject, interactionType, placementPosition, interactionTime,  gameState.GetComponent<MachineHandler>(), destroyMachineOnCompletion);
        }

        if (hasAnimation)
        {
            animator = this.gameObject.GetComponent<Animator>();
            machine.SetAnimator(animator, animationLayer);
        }

        gameState.GetComponent<MachineHandler>().RegisterObject(machine);


    }
}
