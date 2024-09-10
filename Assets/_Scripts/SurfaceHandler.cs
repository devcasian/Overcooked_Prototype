using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceHandler : MonoBehaviour
{
    public List<Surface> objectList = new List<Surface>();

   
    public void RegisterObject(GameObject Object, Vector3 localPosition)
    {
        objectList.Add(new Surface(Object, localPosition));
    }

    public void RegisterObject(Surface inputObject)
    {
        objectList.Add(inputObject);
    }


    public Surface NearestObjectWithinGrabRadius(float grabRadius, Vector3 Position)
    {
        float Distance = 0;
        float minDistance = 100000;

        Surface nearestObject = null;

        foreach (Surface x in objectList)
        {
            Distance = (x.gameObject.transform.position - Position).magnitude;
   
            if (Distance < minDistance && x.surfaceEmpty)
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

public class Surface 
{
    public GameObject gameObject;

    public bool surfaceEmpty;
    public Vector3 localPosition;


    public Surface(GameObject gameObject, Vector3 localPosition)
    {
        this.gameObject = gameObject;
        this.surfaceEmpty = true;
        this.localPosition = localPosition;
    }

}