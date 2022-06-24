using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private PlayerInput inputActions;
    
    private PlayerInputActions playerInputActions;
    

    public float fJumpForce = 5f;
    public float fSpeed = 1f;

    private bool bCanJump = true;
    

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        inputActions = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        rigidbody2D.AddForce(new Vector2(inputVector.x, 0) * fSpeed, ForceMode2D.Impulse);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && bCanJump)
        {
            rigidbody2D.AddForce(Vector2.up * fJumpForce, ForceMode2D.Impulse);
            bCanJump = false;
        }    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Floor")
        {
            bCanJump = true;
        }
    }
}
