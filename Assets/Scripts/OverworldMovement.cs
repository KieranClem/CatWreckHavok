using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldMovement : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private PlayerInput inputActions;
    private PlayerInputActions playerInputActions;

    public float fMovementSpeed = 5f;


    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        inputActions = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.OverworldPlayerMovement.Enable();
    }

    private void FixedUpdate()
    {
        Vector2 inputValue = playerInputActions.OverworldPlayerMovement.Movement.ReadValue<Vector2>();
        rigidbody2D.MovePosition(rigidbody2D.position + inputValue * fMovementSpeed * Time.fixedDeltaTime);

    }
}
