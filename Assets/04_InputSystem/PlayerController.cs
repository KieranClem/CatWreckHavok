using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Stores the mask for the platforms, used to determine if the player is on the ground when dashing
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    
    //Variable stored that are needed for the movement
    private Rigidbody2D rigidbody2D;
    private PlayerInput inputActions;
    private PlayerInputActions playerInputActions;
    
    [Header("Player Movement information")]
    public float fJumpForce = 5f;
    public float fSpeed = 1f;
    public float fMaxSpeed = 5f;

    //info for if the player can jump/double jump
    private bool bCanJump = true;
    private bool bDoubleJump = true;

    //dash information
    public float fDashSpeed = 1f;
    public float fDashTime = 1f;
    private bool bCanDash = true;
    private bool bDashing = false;

    //Stomp information
    public float fStompSpeed = 1f;
    private bool bInStomp = false;

    //access information about the capsule collider
    private CapsuleCollider2D capsuleCollider;

    //Coyote Time Information
    public float CoyoteTime = 0.2f;
    private float CoyoteTimeCounter;
    private bool bCoyoteTimeActive = false;

    //Info for switch characters
    public PlayableCharacter currentCharacter;

    //Stores checkpoint
    private Transform CheckPoint;

    //Stores current character switch
    private CharacterSwitch characterSwitch = null;
    

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        inputActions = GetComponent<PlayerInput>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        
        SwitchCharacter(currentCharacter);

        inputActions.actions["SwitchCharacter"].Disable();
    }

    private void Update()
    {
        if(!bCoyoteTimeActive)
        {
            CoyoteTimeCounter = CoyoteTime;
        }
        else
        {
            CoyoteTimeCounter -= Time.deltaTime;
            if(CoyoteTimeCounter <= 0)
            {
                bCanJump = false;
            }
        }

        //If player lands on top of enemy bounces them up, currently only goes up same height as normaal jump
        if(!bCanJump)
        {
            float extraHeight = 0.01f;
            RaycastHit2D raycastHit = Physics2D.Raycast(capsuleCollider.bounds.center, Vector2.down, capsuleCollider.bounds.extents.y + extraHeight, enemyLayerMask);
            if(raycastHit.collider != null)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                rigidbody2D.AddForce(Vector2.up * fJumpForce, ForceMode2D.Impulse);
            }
        }

        if((rigidbody2D.velocity.magnitude > fMaxSpeed) && !bDashing)
        {
            rigidbody2D.velocity = rigidbody2D.velocity.normalized * fMaxSpeed;
        }
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
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
            rigidbody2D.AddForce(Vector2.up * fJumpForce, ForceMode2D.Impulse);
            bCanJump = false;
        }
        //Double Jump, checks if the player has already performed the double jump
        else if(context.performed && bDoubleJump)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
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

            StopCoroutine(DashCounter());
            StartCoroutine(DashCounter());

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

    IEnumerator DashCounter()
    {
        bDashing = true;
        yield return new WaitForSeconds(fDashTime);
        bDashing = false;
    }

    public void Stomp(InputAction.CallbackContext context)
    {
        //Checks if the player is in the air before being able to stomp
        if(context.performed && !bCanJump)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.velocity = Vector2.down * fStompSpeed;
            bInStomp = true;
        }
    }

    //checks to see if a button has been pressed to switch character
    public void activateCharacterSwitch(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            SwitchCharacter(characterSwitch.characterToSwitchTo);
        }  
    }

    //Deactivates the action that the character doesn't have access to, will need to add sprite change here later on
    private void SwitchCharacter(PlayableCharacter character)
    {
        currentCharacter = character;
        if(currentCharacter == PlayableCharacter.Spring)
        {
            bDoubleJump = true;
            inputActions.actions["Dash"].Disable();
            inputActions.actions["Stomp"].Disable();
        }
        else if (currentCharacter == PlayableCharacter.Dash)
        {
            inputActions.actions["Dash"].Enable();
            inputActions.actions["Stomp"].Disable();
            bDoubleJump = false;
        }
        else if (currentCharacter == PlayableCharacter.Slam)
        {
            inputActions.actions["Stomp"].Enable();
            inputActions.actions["Dash"].Disable();
            bDoubleJump = false;
        }
    }

    private void SendPlayertoCheckPoint()
    {
        this.transform.position = CheckPoint.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks all the possible tags the player can interact with and activates the appropriate code
        switch(collision.tag)
        {
            case "Floor":
                bCanJump = true;
                bCanDash = true;
                if (currentCharacter == PlayableCharacter.Spring)
                {
                    bDoubleJump = true;
                }

                bCoyoteTimeActive = false;
                bInStomp = false;
                break;

            case "CharacterSwitcher":
                inputActions.actions["SwitchCharacter"].Enable();
                characterSwitch = collision.GetComponent<CharacterSwitch>();
                break;

            case "CheckPoint":
                CheckPoint = collision.GetComponent<Transform>();
                break;

            case "DeathZone":
                SendPlayertoCheckPoint();
                break;

            case "BreakableFloor":
                if(!bInStomp)
                {
                    //Acts as normal ground if not in stomp
                    bCanJump = true;
                    bCanDash = true;
                    if (currentCharacter == PlayableCharacter.Spring)
                    {
                        bDoubleJump = true;
                    }

                    bCoyoteTimeActive = false;
                }
                else
                {
                    //Deactivates ground below, later could add animation of ground breaking but currently will just disappear
                    collision.gameObject.SetActive(false);
                }
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Checks all the possible tags the player can interact with and activates the appropriate code
        switch (collision.tag)
        {
            case "Floor":
                bCoyoteTimeActive = true;
                break;

            case "CharacterSwitcher":
                inputActions.actions["SwitchCharacter"].Disable();
                characterSwitch = null;
                break;
        }
    }
}
