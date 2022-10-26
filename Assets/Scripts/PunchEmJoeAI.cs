using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchEmJoeAI : MonoBehaviour
{
    public float fMovementSpeed;
    public float fMaxSpeed;

    private Rigidbody2D rigidbody2D;
    private Transform tPlayerLocation;
    
    // Start is called before the first frame update
    void Start()
    {
        tPlayerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
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

    private void FixedUpdate()
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
