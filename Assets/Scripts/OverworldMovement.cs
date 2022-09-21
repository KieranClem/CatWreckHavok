using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldMovement : MonoBehaviour
{
    //Variables
    private Rigidbody2D rigidbody2D;
    private PlayerInput inputActions;
    private PlayerInputActions playerInputActions;

    private bool bNextToEntrance = false;
    private LoadLevel loadLevel;

    public float fMovementSpeed = 5f;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        inputActions = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.OverworldPlayerMovement.Enable();
        bNextToEntrance = false;
}

    private void FixedUpdate()
    {
        //Player movement, get input and move accordingly
        Vector2 inputValue = playerInputActions.OverworldPlayerMovement.Movement.ReadValue<Vector2>();
        rigidbody2D.MovePosition(rigidbody2D.position + inputValue * fMovementSpeed * Time.fixedDeltaTime);

    }

    //Interact with level entrance
    public void Interact(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            //Checks if player is next to enterance point
            if (bNextToEntrance)
            {
                //Send to next level
                loadLevel.LoadNextLevel();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks if next to level entrance, allows player to interact by storing the entrance
        if(collision.tag == "LevelEntrance")
        {
            bNextToEntrance = true;
            loadLevel = collision.GetComponent<LoadLevel>();
            Debug.Log(inputActions.actions["Select"].enabled);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Checks if has left entrance, if they have remove it and prevent the player from trying to load a new level
        if (collision.tag == "LevelEntrance")
        {
            bNextToEntrance = false;
            inputActions.actions["Select"].Disable();
            loadLevel = null;
            Debug.Log(inputActions.actions["Select"].enabled);
        }
    }
}
