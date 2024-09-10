using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    public List<MovableObject> objectList = new List<MovableObject>();

   
    public void RegisterObject(GameObject inputObject, string interactionType)
    {
        objectList.Add(new MovableObject(inputObject, interactionType));
    }

    public void RegisterObject(MovableObject inputObject)
    {
        objectList.Add(inputObject);
    }

    public void RemoveObject(MovableObject inputObject)
    {
        objectList.Remove(inputObject);
    }

    public MovableObject CreateAndRegisterObject(GameObject inputObject)
    {
        Instantiate(inputObject);

        return objectList[objectList.Count - 1];
    }


   
    public MovableObject NearestObjectWithinGrabRadius(float grabRadius, Vector3 Position)
    {
        float Distance = 0;
        float minDistance = 100000;

        MovableObject nearestObject = null;

        foreach (MovableObject x in objectList)
        {
            Distance = (x.gameObject.transform.position - Position).magnitude;

            if (Distance < minDistance && x.freeToGrab)
            {
                minDistance = Distance;
                nearestObject = x;
            }
        }

        if (minDistance < grabRadius)
            return nearestObject;  
        else
            return null;
    }

}

 /// TO DO ///
 // rename to MovableObject
public class MovableObject 
{
    public GameObject hands;
    public GameObject gameObject;

    public string interactionType;
    public bool freeToGrab;
    public bool taskCompleted;
    public Surface surface;

    public Machine machine;



    public MovableObject(GameObject gameObject, string interactionType)//, ObjectHandler objectHandler)
    {
        this.gameObject = gameObject;
        this.hands = gameObject.transform.GetChild(1).gameObject;
        this.interactionType = interactionType;
        this.freeToGrab = true;
        this.machine = null;
        this.surface = null;
    }

}
