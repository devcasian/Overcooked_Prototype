using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField]
    Transform player;

    [SerializeField]
    float accelerationScaling = 1f;

    [SerializeField]
    float rotateSpeed = 10f;

    [SerializeField]
    float tiltSpeed = 10f;

    [SerializeField]
    float maxSpeed = 10f;

    Vector3 acceleration = new Vector3(0f,0f,0f);
    Vector2 angleVector = new Vector2(0f, 0f);
    Vector2 lastNonZeroVelocity = new Vector2(0f, 0f);
    Vector3 velocity;

    Rigidbody body;

    GameObject leftArm;
    GameObject rightArm;

    GameObject throwIndicator;

    float angle;
    float tiltAngle;

    MovableObject pickedUpObject = null;

    GameObject gameState;   

    void Awake()
    {
        body = player.GetComponent<Rigidbody>();
        gameState = GameObject.Find("Game State");

        leftArm = player.transform.Find("arm0").gameObject;
        rightArm = player.transform.Find("arm1").gameObject;

        throwIndicator = player.transform.Find("Throw Indicator").gameObject;

        leftArm.GetComponentInChildren<LineRenderer>().SetPosition(0, player.transform.GetChild(1).position);
        rightArm.GetComponentInChildren<LineRenderer>().SetPosition(0, player.transform.GetChild(2).position);

        leftArm.GetComponentInChildren<LineRenderer>().SetPosition(1, leftArm.transform.Find("Sphere").transform.position);
        rightArm.GetComponentInChildren<LineRenderer>().SetPosition(1, rightArm.transform.Find("Sphere").transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        acceleration.x = Input.GetAxis("Horizontal");
        acceleration.y = Input.GetAxis("Vertical");

        // Normalize the input vector to make controls more uniform and scale accordingly.
        acceleration.Normalize();
        
        if (Mathf.Abs(acceleration.x) > 0.1f || Mathf.Abs(acceleration.y) > 0.1f)
        {
            angle = -1 * AngleFromUnitCirclePosition(acceleration.x, acceleration.y);

            tiltAngle = 10f;
        }
        else
        {
            tiltAngle = 0f;
        }

        acceleration *= accelerationScaling;

        // Pickup or drop the closest item (if within the drop radius).
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (pickedUpObject == null)
            {
                PickUpObject(2.5f);
            }
            else
            {
                var nearsestSurface = gameState.GetComponent<SurfaceHandler>().NearestObjectWithinGrabRadius(2.5f, player.transform.position);

                if (nearsestSurface == null)
                    DropObject();
                else 
                    PlaceOnSurface(nearsestSurface);
            }
        }

        // Throw the item if picked-up.
        if (Input.GetKeyDown("z"))
        {
            if (pickedUpObject != null)
            {
                DisplayThrowIndicator();
            }
        }

        if (Input.GetKeyUp("z"))
        {
            if (pickedUpObject != null)
            {
                DropObject(10f);
                HideThrowIndicator();
            }
        }

        if (Input.GetKeyDown("x"))
        {
            if (pickedUpObject != null)
            {
                FillMachine(2.5f);
            }
        }

        UpdateArms();

    }

    void FixedUpdate()
    {
        //Get the current velocity from the rigidbody.
        velocity = body.velocity;

        //Get and clamp the new velocity.
        velocity.x += Time.deltaTime * acceleration.x;
        velocity.z += Time.deltaTime * acceleration.y;

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        //Update the velocity of the solidbody.
        body.velocity = velocity;

        Vector3 currentAngle = player.transform.localEulerAngles;

        currentAngle.y = Mathf.MoveTowardsAngle(currentAngle.y, angle, rotateSpeed * Time.deltaTime);
        currentAngle.z = tiltAngle != 0 ? Mathf.MoveTowardsAngle(currentAngle.z, tiltAngle, tiltSpeed * Time.deltaTime) : 0;
        currentAngle.x = 0;

        player.transform.localEulerAngles = currentAngle;

        // If item is picked up co-rotate the item with the player.
        if (pickedUpObject != null)
        {
            pickedUpObject.gameObject.transform.position = player.position;
            pickedUpObject.gameObject.transform.localEulerAngles = new Vector3(0f, currentAngle.y, 0f);
        }
    }

    void DropObject(float speed = 0f)
    {
        pickedUpObject.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
        pickedUpObject.gameObject.GetComponent<Collider>().enabled = true;
        pickedUpObject.gameObject.GetComponent<Rigidbody>().useGravity = true;

        pickedUpObject.gameObject.GetComponent<Rigidbody>().velocity = player.GetComponent<Rigidbody>().velocity + (pickedUpObject.gameObject.transform.right + new Vector3(0f, .2f, 0f)) * speed;

        pickedUpObject.gameObject.transform.position = pickedUpObject.gameObject.transform.GetChild(0).position;

        pickedUpObject.gameObject.transform.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
        pickedUpObject.gameObject.transform.GetChild(1).localPosition = new Vector3(0f, 0f, 0f);

        pickedUpObject.gameObject.transform.GetChild(1).gameObject.SetActive(false);


        pickedUpObject = null;
    }

    void PlaceOnSurface(Surface surface)
    {
        pickedUpObject.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
        pickedUpObject.gameObject.GetComponent<Collider>().enabled = false;
        pickedUpObject.gameObject.GetComponent<Rigidbody>().useGravity = false;

        pickedUpObject.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);

        pickedUpObject.gameObject.transform.position = surface.gameObject.transform.position + surface.localPosition;

        pickedUpObject.gameObject.transform.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
        pickedUpObject.gameObject.transform.GetChild(1).localPosition = new Vector3(0f, 0f, 0f);

        pickedUpObject.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        pickedUpObject.surface = surface;

        surface.surfaceEmpty = false;

        //print("test");

        //print(pickedUpObject.machine.gameObject.name);

        /*
        if (pickedUpObject.machine != null)
        {
            print("yay");
            pickedUpObject.machine.machineFilled = false; 
        }
        */

        pickedUpObject = null;
    }

    void PickUpObject(float pickupRadius)
    {
        MovableObject nearestObject = gameState.GetComponent<ObjectHandler>().NearestObjectWithinGrabRadius(pickupRadius, player.transform.position);
        Dispenser nearestDispenser = gameState.GetComponent<DispenserHandler>().NearestDispenserWithinGrabRadius(pickupRadius, player.transform.position);

        if (nearestObject == null && nearestDispenser == null)
            return;

        if (nearestObject != null && nearestDispenser == null)
        {
            pickedUpObject = nearestObject;

            if (nearestObject.machine != null)
            {
                nearestObject.machine.EmptyMachine();
                nearestObject.machine = null;
            }
        }
        else if (nearestObject == null && nearestDispenser != null)
        {
            pickedUpObject = nearestDispenser.DispenseObject();
        }
        else if (nearestObject != null && nearestDispenser != null)
        {
            if ((nearestObject.gameObject.transform.position - player.position).magnitude < (nearestDispenser.gameObject.transform.position - player.position).magnitude)
            {
                pickedUpObject = nearestObject;

                if (nearestObject.machine != null)
                {
                    nearestObject.machine.EmptyMachine();
                    nearestObject.machine = null;
                }
            }
            else
            {
                pickedUpObject = nearestDispenser.DispenseObject();
            }
        }

        pickedUpObject.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
        pickedUpObject.gameObject.GetComponent<Collider>().enabled = false;
        pickedUpObject.gameObject.GetComponent<Rigidbody>().useGravity = false;

        if (pickedUpObject.surface != null)
        {
            pickedUpObject.surface.surfaceEmpty = true;
            pickedUpObject.surface = null;
        }
        //if (pickedUpObject.machine != null) {pickedUpObject.machine.machineFilled = true; }

        pickedUpObject.gameObject.transform.position = player.position + new Vector3(1.5f, 0.5f, 0f);
        pickedUpObject.gameObject.transform.GetChild(0).localPosition = new Vector3(1.5f, 0.5f, 0f);
        pickedUpObject.gameObject.transform.GetChild(1).localPosition = new Vector3(1.5f, 0.5f, 0f);      
    }

    void UpdateArms()
    {
        if (pickedUpObject != null)
        {
            leftArm.GetComponentInChildren<LineRenderer>().SetPosition(0, player.transform.GetChild(1).position);
            rightArm.GetComponentInChildren<LineRenderer>().SetPosition(0, player.transform.GetChild(2).position);

            leftArm.GetComponentInChildren<LineRenderer>().SetPosition(1, pickedUpObject.gameObject.transform.GetChild(1).GetChild(0).position);
            rightArm.GetComponentInChildren<LineRenderer>().SetPosition(1, pickedUpObject.gameObject.transform.GetChild(1).GetChild(1).position);

            leftArm.transform.GetChild(1).gameObject.SetActive(false);
            rightArm.transform.GetChild(1).gameObject.SetActive(false);

            pickedUpObject.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            leftArm.GetComponentInChildren<LineRenderer>().SetPosition(0, player.transform.GetChild(1).position);
            rightArm.GetComponentInChildren<LineRenderer>().SetPosition(0, player.transform.GetChild(2).position);


            leftArm.GetComponentInChildren<LineRenderer>().SetPosition(1, leftArm.transform.Find("Sphere").transform.position);
            rightArm.GetComponentInChildren<LineRenderer>().SetPosition(1, rightArm.transform.Find("Sphere").transform.position);


            leftArm.transform.GetChild(1).gameObject.SetActive(true);
            rightArm.transform.GetChild(1).gameObject.SetActive(true);
        }

    }

    void FillMachine(float interactionRadius)
    {

        var nearestMachines = gameState.GetComponent<MachineHandler>().MachinesWithinGrabRadius(interactionRadius, player.transform.position);

        if (nearestMachines == null)
            return;
        
 
        foreach (Machine x in nearestMachines)
        {
            if (pickedUpObject.interactionType == x.interactionType)
            {
                pickedUpObject.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
                pickedUpObject.gameObject.GetComponent<Rigidbody>().useGravity = false;
                pickedUpObject.gameObject.GetComponent<Collider>().enabled = false;
                pickedUpObject.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);

                x.PlaceObject(pickedUpObject);

                pickedUpObject = null;

                break;
            }    

     
        }
        
    }

    void DisplayThrowIndicator()
    {
        throwIndicator.SetActive(true);
    }

    void HideThrowIndicator()
    {
        throwIndicator.SetActive(false);
    }

    float CalculateThrowDistance(float speed, Vector3 angle)
    {
        return 0.0f;
    }

    float AngleFromUnitCirclePosition(float x, float y)
    {
        if (x > 0f && y > 0f)
            return Mathf.Asin(y) * 180f/Mathf.PI;

        if (x > 0f && y < 0f)
            return 360f - Mathf.Asin(-y) * 180f/Mathf.PI;

        if (x < 0f && y > 0f)
            return 180f - Mathf.Asin(y) * 180f/Mathf.PI;

        if (x < 0f && y < 0f)
            return Mathf.Asin(-y) * 180f/Mathf.PI + 180f;

        if (y == 0f && x > 0f)
            return 0f;

        if (y == 0f && x < 0f)
            return 180f;

        if (y > 0f && x == 0f)
            return 90f;

        if (y < 0f && x == 0f)
            return 270f;

        return 0f;
    }

}
