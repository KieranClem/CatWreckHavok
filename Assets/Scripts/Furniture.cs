using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    //Tracks if the player has knocked it down
    [HideInInspector] public bool bKnockedDown = false;
    //Furniture manager is stored here for access, this variable is set in the furniture manager script in the awake method
    [HideInInspector] public FurnitureManager furnitureManager;
    //Stores furnitures default position
    public Vector3 fDefaultPosition;

    private void Awake()
    {
        fDefaultPosition = this.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the furniture collides with the player, the furniture will be added to the ruined furniture and be knocked down
        if(collision.tag == "Player" && !bKnockedDown)
        {
            furnitureManager.addRuinedFurniture(this);
            bKnockedDown = true;
            this.gameObject.SetActive(false);
        }
    }
}
