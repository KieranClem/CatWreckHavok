using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownItem : MonoBehaviour
{
    public MuscleSteveAI muscleSteveAI;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(new Vector3(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z + 100) * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Floor"))
        {
            muscleSteveAI.ResetThrownItemBooleans();
            Destroy(this.gameObject);
        }
    }
}
