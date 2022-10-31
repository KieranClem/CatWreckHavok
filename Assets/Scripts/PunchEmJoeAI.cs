using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchEmJoeAI : MonoBehaviour
{
    
    [Header("Movement Info")]
    public float fMovementSpeed;
    public float fMaxSpeed;

    [Header("Phase 1 Charge Info")]
    public float fPhase1ChargeTimeBeforeLockingDirection = 5f;
    public float fPhase1ChargeTimeAfterLockingDirection = 2.5f;
    public float fPhase1ChargeAttackDuration = 2.5f;
    public float fPhase1ChargeAttackForce = 100f;

    [Header("Phase 2 Charge Info")]
    public float fPhase2ChargeTimeBeforeLockingDirection = 5f;
    public float fPhase2ChargeTimeAfterLockingDirection = 2.5f;
    public float fPhase2ChargeAttackDuration = 2.5f;
    public float fPhase2ChargeAttackForce = 100f;

    [Header("Phase 3 Charge Info")]
    public float fPhase3ChargeTimeBeforeLockingDirection = 5f;
    public float fPhase3ChargeTimeAfterLockingDirection = 2.5f;
    public float fPhase3ChargeAttackDuration = 2.5f;
    public float fPhase3ChargeAttackForce = 100f;

    private Rigidbody2D rigidbody2D;
    private Transform tPlayerLocation;

    private int PhaseNumber = 1;

    private bool bIsCharging;
    private bool bFinishedCharging = false;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        bIsCharging = false;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        tPlayerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!bIsCharging)
        {
            //Has a max speed, prevents players from going too fast to the right
            if ((rigidbody2D.velocity.x > fMaxSpeed))
            {
                rigidbody2D.velocity = new Vector2(fMaxSpeed, rigidbody2D.velocity.y);
            }
            //Has a max speed, prevents players from going too fast to the left
            if (rigidbody2D.velocity.x < -fMaxSpeed)
            {
                rigidbody2D.velocity = new Vector2(-fMaxSpeed, rigidbody2D.velocity.y);
            }
        }


        if (!bFinishedCharging)
        {
            //Sprite look right
            if (tPlayerLocation.position.x > this.transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            //Sprite look left
            else if (tPlayerLocation.position.x < this.transform.position.x)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!bIsCharging)
        {
            //Follow the player
            if (tPlayerLocation.position.x > this.transform.position.x)
            {
                rigidbody2D.AddForce(new Vector2(1, 0) * fMovementSpeed, ForceMode2D.Impulse);
            }
            else if (tPlayerLocation.position.x < this.transform.position.x)
            {
                rigidbody2D.AddForce(new Vector2(-1, 0) * fMovementSpeed, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //Checks if the player jumped on the enemies head
            if (transform.position.y < collision.transform.position.y)
            {
                //Different charge information depending on the phase
                switch (PhaseNumber)
                {
                    case 1:
                        StartCoroutine(StartCharging(fPhase1ChargeTimeBeforeLockingDirection, fPhase1ChargeTimeAfterLockingDirection, fPhase1ChargeAttackDuration, fPhase1ChargeAttackDuration));
                        break;
                    case 2:
                        StartCoroutine(StartCharging(fPhase2ChargeTimeBeforeLockingDirection, fPhase2ChargeTimeAfterLockingDirection, fPhase2ChargeAttackDuration, fPhase2ChargeAttackDuration));
                        break;
                    case 3:
                        StartCoroutine(StartCharging(fPhase3ChargeTimeBeforeLockingDirection, fPhase3ChargeTimeAfterLockingDirection, fPhase3ChargeAttackDuration, fPhase3ChargeAttackDuration));
                        break;
                }
            }
            else
            {
                //reset or knock player back here
            }
        }
    }

    IEnumerator StartCharging(float FirstWaitTime, float SecondWaitTime, float AttackDuration, float AttackForce)
    {
        bIsCharging = true;

        yield return new WaitForSeconds(FirstWaitTime);

        bool LeftRight = true;
        
        if(tPlayerLocation.position.x > this.transform.position.x)
        {
            LeftRight = true;
        }
        else if (tPlayerLocation.position.x < this.transform.position.x)
        {
            LeftRight = false;
        }
        bFinishedCharging = true;

        yield return new WaitForSeconds(SecondWaitTime);

        if(LeftRight)
        {
            rigidbody2D.AddForce(new Vector2(1, 0) * AttackForce, ForceMode2D.Impulse);
        }
        else
        {
            rigidbody2D.AddForce(new Vector2(-1, 0) * AttackForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(AttackDuration);

        rigidbody2D.velocity = Vector2.zero;
        bIsCharging = false;
        bFinishedCharging = false;
    }
}
