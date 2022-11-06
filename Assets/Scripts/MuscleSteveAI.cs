using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleSteveAI : MonoBehaviour
{

    //Thrown Item information
    public GameObject ThrownItem;
    private GameObject SpawnedItem;
    public Transform tThrowHighLocation;
    public float ThrownSpeed;
    private bool bItemSpawned = false;
    private bool bReachedTop = false;

    private GameObject Player;
    private bool bMovingToPlayer = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!bItemSpawned)
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
}
