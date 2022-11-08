using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleSteveAI : MonoBehaviour
{
    [Header("Thrown Item Info")]
    //Thrown Item information
    public GameObject ThrownItem;
    private GameObject SpawnedItem;
    public Transform tThrowHighLocation;
    public float ThrownSpeed;
    private bool bItemSpawned = false;
    private bool bReachedTop = false;

    [Header("Health Info")]
    public int iMaxHealth = 3;
    private int iCurrentHealth;

    [Header("Platform that needs to be destroyed")]
    public GameObject Platform;

    private PunchEmJoeAI PunchEmJoeAI;

    private GameObject Player;
    private bool bMovingToPlayer = false;

    private bool bFalling = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").gameObject;
        PunchEmJoeAI = GameObject.FindGameObjectWithTag("PunchEmJoe").GetComponent<PunchEmJoeAI>();
        iCurrentHealth = iMaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!bItemSpawned && !bFalling)
        {
            SpawnedItem = Instantiate(ThrownItem, new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z), Quaternion.identity);
            SpawnedItem.GetComponent<ThrownItem>().muscleSteveAI = this;
            bItemSpawned = true;
        }
        else
        {
            if (!bReachedTop)
            {
                SpawnedItem.transform.position = Vector3.MoveTowards(SpawnedItem.transform.position, tThrowHighLocation.position, ThrownSpeed * Time.deltaTime);

                if (SpawnedItem.transform.position == tThrowHighLocation.position)
                {
                    bReachedTop = true;
                }
            }
            else
            {
                if(!bMovingToPlayer)
                {
                    SpawnedItem.transform.position = new Vector3(Player.transform.position.x, SpawnedItem.transform.position.y, SpawnedItem.transform.position.z);
                    bMovingToPlayer = true;
                    SpawnedItem.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -ThrownSpeed), ForceMode2D.Impulse);
                }
            }
        }
    }

    public void ResetThrownItemBooleans()
    {
        bItemSpawned = false;
        bReachedTop = false;
        bMovingToPlayer = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == Player)
        {
            if(Player.transform.position.y > this.transform.position.y)
            {
                if(Platform != null)
                {
                    Destroy(Platform);
                    TakeDamage();
                    bFalling = true;
                }
                PunchEmJoeAI.ChangePhase();
            }
            else
            {

            }
        }

        if(collision.tag == "Floor")
        {
            bFalling = false;
        }
    }

    public void TakeDamage()
    {
        iCurrentHealth -= 1;

        if(iCurrentHealth == 0)
        {
            //Defeat information goes here
        }
        else if(iCurrentHealth != (iMaxHealth - 1))
        {
            PushPunchEmJoeBack();
        }
    }

    public void PushPunchEmJoeBack()
    {
        PunchEmJoeAI.GetComponent<Rigidbody2D>().AddForce(new Vector2(ThrownSpeed, 0), ForceMode2D.Impulse);
    }
}
