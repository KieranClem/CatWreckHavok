using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Stores the mask for the platforms, used to determine if the player is on the ground when dashing
    [SerializeField] private LayerMask platformLayerMask;
    
    //Variable stored that are needed for the movement
    private Rigidbody2D rigidbody2D;
    private PlayerInput inputActions;
    private PlayerInputActions playerInputActions;
    
    [Header("Player Movement information")]
    public float fJumpForce = 5f;
    public float fSpeed = 1f;

    //info for if the player can jump/double jump
    private bool bCanJump = true;
    private bool bDoubleJump = true;

    //dash information
    public float fDashSpeed = 1f;
    private bool bCanDash = true;

    //Stomp information
    public float fStompSpeed = 1f;

    //access information about the capsule collider
    private CapsuleCollider2D capsuleCollider;
    

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        inputActions = GetComponent<PlayerInput>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Basic movement, gets the player inputs and moves them in the direction they pressed
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        rigidbody2D.AddForce(new Vector2(inputVector.x, 0) * fSpeed, ForceMode2D.Impulse);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //Normal Jump, checks if the player has already jumped
        if(context.performed && bCanJump)
        {
            rigidbody2D.AddForce(Vector2.up * fJumpForce, ForceMode2D.Impulse);
            bCanJump = false;
        }
        //Double Jump, checks if the player has already performed the double jump
        else if(context.performed && bDoubleJump)
        {
            rigidbody2D.AddForce(Vector2.up * fJumpForce, ForceMode2D.Impulse);
            bDoubleJump = false;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed && bCanDash)
        {
            //Stores player's input
            Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();

            //Checks which direction the player is dashing in
            if(inputVector.x > 0)
            {
                rigidbody2D.velocity = Vector2.right * fDashSpeed;
            }
            else if (inputVector.x < 0)
            {
                rigidbody2D.velocity = Vector2.left * fDashSpeed;
            }
            else
            {
                Debug.Log("wasn't moving");
                //will need to figure out how to handle dash when the player isn't moving, might look to store the last direction input they had but not sure
            }

            //checks if the player is still on the ground, allows them to keep dashing if they are
            float extraHeight = 0.01f;
            RaycastHit2D raycastHit = Physics2D.Raycast(capsuleCollider.bounds.center, Vector2.down, capsuleCollider.bounds.extents.y + extraHeight, platformLayerMask);
            if(raycastHit.collider != null)
            {
                bCanDash = true;
            }
            else
            {
                bCanDash = false;
            }
        }
    }

    public void Stomp(InputAction.CallbackContext context)
    {
        //Checks if the player is in the air before being able to stomp
        if(context.performed && !bCanJump)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.velocity = Vector2.down * fStompSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //resets jumps and other information once the player has landed
        if(collision.tag == "Floor")
        {
            bCanJump = true;
            bDoubleJump = true;
            bCanDash = true;
        }
    }
}
