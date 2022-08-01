using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    //Stores the mask for the platforms, used to determine if the player is on the ground when dashing
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    
    //Variable stored that are needed for the movement
    private Rigidbody2D rigidbody2D;
    private PlayerInput inputActions;
    private PlayerInputActions playerInputActions;

    [Header("Select Character")]
    //Info for switch characters
    public PlayableCharacter currentCharacter;

    //Character's unique stats, not used in the actual calculations, just used to set variables that are actually used in the calculations
    [Header("Spring's Movement")]
    public float fSpringJumpForce = 5f;
    public float fSpringMovementSpeed = 5f;
    public float fSpringMaxSpeed = 10f;
    public float fSpringDrag = 1f;
    public float fSpringBounce = 1f;

    [Header("Dash's Movement")]
    public float fDashJumpForce = 5f;
    public float fDashMovementSpeed = 5f;
    public float fDashMaxSpeed = 10f;
    public float fDashDrag = 1f;
    //Boolean used to store the direction the player was facing during movement, used to know which direction to dash after movement has stopped
    private bool LeftRightDash = true;

    [Header("Slam's Movement")]
    public float fSlamJumpForce = 5f;
    public float fSlamMovementSpeed = 5f;
    public float fSlamMaxSpeed = 5f;
    public float fSlamDrag = 1f;

    //Variables that will actually be used
    private float fJumpForce = 5f;
    private float fSpeed = 1f;
    private float fMaxSpeed = 5f;

    //info for if the player can jump/double jump
    private bool bCanJump = true;
    private bool bDoubleJump = true;

    [Header("Ability information")]
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

    private BoxCollider2D BoxCollider2D;

    //Coyote Time Information
    public float CoyoteTime = 0.2f;
    private float CoyoteTimeCounter;
    private bool bCoyoteTimeActive = false;

    //Stores checkpoint
    private Transform CheckPoint;

    //Stores current character switch
    private CharacterSwitch characterSwitch = null;

    //Animation variables
    private Animator animator;
    public RuntimeAnimatorController SpringAnimationController;
    public RuntimeAnimatorController DashAnimationController;
    public RuntimeAnimatorController SlamAnimationController;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        inputActions = GetComponent<PlayerInput>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
        playerInputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
        playerInputActions.Player.Enable();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
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
            float extraHeight = 0.1f;
            if (currentCharacter == PlayableCharacter.Spring)
            {
                RaycastHit2D raycastHit = Physics2D.Raycast(capsuleCollider.bounds.center, Vector2.down, capsuleCollider.bounds.extents.y + extraHeight, enemyLayerMask);
                if (raycastHit.collider != null)
                {
                    Debug.Log("here");

                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                    rigidbody2D.AddForce(Vector2.up * fSpringBounce, ForceMode2D.Impulse);
                }
            }

            float extraHeightLanding = 0.01f;
            //Checks if the player hits the floor
            RaycastHit2D JumpRaycastHit = Physics2D.Raycast(capsuleCollider.bounds.center, Vector2.down, capsuleCollider.bounds.extents.y + extraHeightLanding, platformLayerMask);
            if(JumpRaycastHit.collider != null)
            {
                bCanDash = true;
                bCanJump = true;
                bCoyoteTimeActive = false;
                bInStomp = false;
                if (currentCharacter == PlayableCharacter.Spring)
                {
                    bDoubleJump = true;
                }
                animator.SetBool("IsJumping", false);
                if (currentCharacter == PlayableCharacter.Slam)
                {
                    animator.SetBool("isStomping", false);
                }

            }
        }

        if(bInStomp)
        {
            float extraHeight = 1f;
            if (LeftRightDash)
            {
                RaycastHit2D raycastHit = Physics2D.Raycast(BoxCollider2D.bounds.center, Vector2.right, BoxCollider2D.bounds.extents.x + extraHeight, enemyLayerMask);
                if(raycastHit.collider != null)
                {
                    raycastHit.collider.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.right * (fStompSpeed/2);
                }
            }
            else
            {
                RaycastHit2D raycastHit = Physics2D.Raycast(BoxCollider2D.bounds.center, Vector2.left, BoxCollider2D.bounds.extents.x + extraHeight, enemyLayerMask);
                if (raycastHit.collider != null)
                {
                    raycastHit.collider.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.left * (fStompSpeed / 2);
                }
            }

            
        }

        if ((rigidbody2D.velocity.x > fMaxSpeed) && !bDashing)
        {
            rigidbody2D.velocity = new Vector2(fMaxSpeed, rigidbody2D.velocity.y);
        }

        if(rigidbody2D.velocity.x < -fMaxSpeed && !bDashing)
        {
            rigidbody2D.velocity = new Vector2(-fMaxSpeed, rigidbody2D.velocity.y);
        }

        if(bDashing)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Basic movement, gets the player inputs and moves them in the direction they pressed
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        rigidbody2D.AddForce(new Vector2(inputVector.x, 0) * fSpeed, ForceMode2D.Impulse);
        if (inputVector.x > 0)
        {
            LeftRightDash = true;
            spriteRenderer.flipX = false;
            if (bCanJump)
            {
                animator.SetBool("IsMoving", true);
            }
        }
        else if (inputVector.x < 0)
        {
            LeftRightDash = false;
            spriteRenderer.flipX = true;
            if (bCanJump)
            {
                animator.SetBool("IsMoving", true);
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }


    }

    public void Jump(InputAction.CallbackContext context)
    {
        //Normal Jump, checks if the player has already jumped
        if(context.performed && bCanJump)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
            rigidbody2D.AddForce(Vector2.up * fJumpForce, ForceMode2D.Impulse);
            //bCanJump = false;
            animator.SetBool("IsJumping", true);
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
                //Dash when the player hasn't been moving
                if(LeftRightDash)
                    rigidbody2D.velocity = Vector2.right * fDashSpeed;
                else
                    rigidbody2D.velocity = Vector2.left * fDashSpeed;
            }

            animator.SetBool("IsDashing", true);

            //Stops previous dash counter if active
            StopCoroutine(DashCounter());
            //Starts new one
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
        animator.SetBool("IsDashing", false);
    }

    public void Stomp(InputAction.CallbackContext context)
    {
        //Checks if the player is in the air before being able to stomp
        if(context.performed)
        {
            if (bCanJump)
            {
                Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
                if (inputVector.x > 0)
                {
                    rigidbody2D.velocity = Vector2.zero;
                    rigidbody2D.velocity = Vector2.right * fStompSpeed;
                    bInStomp = true;
                    animator.SetBool("isStomping", true);
                }
                else if (inputVector.x < 0)
                {
                    rigidbody2D.velocity = Vector2.zero;
                    rigidbody2D.velocity = Vector2.left * fStompSpeed;
                    bInStomp = true;
                    animator.SetBool("isStomping", true);
                }
                StartCoroutine(SlamCounter());
            }
            else
            {
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.velocity = Vector2.down * fStompSpeed;
                bInStomp = true;
                animator.SetBool("isStomping", true);
            }

        }
    }

    IEnumerator SlamCounter()
    {

        yield return new WaitForSeconds(1f);
        bInStomp = false;
        animator.SetBool("isStomping", false);
    }

    //checks to see if a button has been pressed to switch character
    public void activateCharacterSwitch(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            PlayableCharacter storeOld = currentCharacter;
            SwitchCharacter(characterSwitch.characterToSwitchTo);
            characterSwitch.characterToSwitchTo = storeOld;
        }  
    }

    //Deactivates the action that the character doesn't have access to, will need to add sprite change here later on
    private void SwitchCharacter(PlayableCharacter character)
    {
        currentCharacter = character;
        if (currentCharacter == PlayableCharacter.Spring)
        {
            bDoubleJump = true;
            inputActions.actions["Dash"].Disable();
            inputActions.actions["Stomp"].Disable();

            if (SpringAnimationController != null) 
            { 
                animator.runtimeAnimatorController = SpringAnimationController;
            }

            fJumpForce = fSpringJumpForce;
            fSpeed = fSpringMovementSpeed;
            fMaxSpeed = fSpringMaxSpeed;
            rigidbody2D.drag = fSpringDrag;
        }
        else if (currentCharacter == PlayableCharacter.Dash)
        {
            inputActions.actions["Dash"].Enable();
            inputActions.actions["Stomp"].Disable();
            bDoubleJump = false;

            if (DashAnimationController != null)
            {
                animator.runtimeAnimatorController = DashAnimationController;
            }

            fJumpForce = fDashJumpForce;
            fSpeed = fDashMovementSpeed;
            fMaxSpeed = fDashMaxSpeed;
            rigidbody2D.drag = fDashDrag;
        }
        else if (currentCharacter == PlayableCharacter.Slam)
        {
            inputActions.actions["Stomp"].Enable();
            inputActions.actions["Dash"].Disable();
            bDoubleJump = false;

            if (SlamAnimationController != null)
            {
                animator.runtimeAnimatorController = SlamAnimationController;
            }

            fJumpForce = fSlamJumpForce;
            fSpeed = fSlamMovementSpeed;
            fMaxSpeed = fSlamMaxSpeed;
            rigidbody2D.drag = fSlamDrag;
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
            /*case "Floor":
                bCanJump = true;
                bCanDash = true;
                if (currentCharacter == PlayableCharacter.Spring)
                {
                    bDoubleJump = true;
                }

                bCoyoteTimeActive = false;
                bInStomp = false;
                break;*/

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
                    animator.SetBool("IsJumping", false);
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "PressurePlate")
        {
            if(collision.transform.position.y <= transform.position.y)
            {
                collision.GetComponent<PressurePlate>().bPressureActivated = true;
            }
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
            case "BreakableFloor":
                bCoyoteTimeActive = true;
                break;
            case "CharacterSwitcher":
                inputActions.actions["SwitchCharacter"].Disable();
                characterSwitch = null;
                break;
        }
    }
}
