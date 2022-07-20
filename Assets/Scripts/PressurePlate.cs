using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Door to open info")]
    public Transform DoorToMove;
    public float DistanceDoorMoves;
    private bool bOpenDoor = false;
    private bool bDoorReachedPosition = false;
    private Vector2 OriginalPositionPlate;

    [Header("Pressure Plate info")]
    public float fDistancePlateWillMove = 5f;
    private bool bReachedPosition = false;  
    private Vector2 OriginalPositionDoor;
    [HideInInspector] public bool bPressureActivated;



    // Start is called before the first frame update
    void Start()
    {
        //Stores original positions
        OriginalPositionPlate = this.transform.position;
        OriginalPositionDoor = DoorToMove.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Move pressure plate down
        if(bPressureActivated && !bReachedPosition)
        {
            transform.Translate(Vector2.down * Time.deltaTime, Space.Self);
            //Checks if it reached the destination
            if(transform.position.y <= (OriginalPositionPlate.y - fDistancePlateWillMove))
            {
                bReachedPosition = true;
                bOpenDoor = true;
            }
        }

        //Move door up
        if(bOpenDoor && !bDoorReachedPosition)
        {
            DoorToMove.Translate(Vector2.up * Time.deltaTime, Space.World);
            //Checks if it reached the destination
            if (DoorToMove.position.y >= (OriginalPositionDoor.y + DistanceDoorMoves))
            {
                bDoorReachedPosition = true;
            }
        }
    }


}
