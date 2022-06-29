using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    //Lists to store the furniture and ruined furniture in for tracking purposes
    private List<Furniture> sceneFurniture = new List<Furniture>();
    private List<Furniture> ruinedFurniture = new List<Furniture>();
    //Number of furntiture needed to finish the level which can be changed in the inspector, default is 1
    public int iNumOfFurnitureForLevelFinish = 1;

    private void Awake()
    {
        //Gets all the gameobjects on the scene with the furniture tag and adds them to the scene furniture list
        foreach (GameObject furn in GameObject.FindGameObjectsWithTag("Furniture"))
        {
            Furniture furniture = furn.GetComponent<Furniture>();
            sceneFurniture.Add(furniture);
            //Stores the furniture manager in the furniture's script for easy access
            furniture.furnitureManager = this;
        }
    }

    //called when the player knocks down some furniture to add to the ruined furniture list
    public void addRuinedFurniture(Furniture ruinFurniture)
    {
        ruinedFurniture.Add(ruinFurniture);
        //Checks if the player has knocked down enough furniture by counting the amount of game objects in the list
        if(ruinedFurniture.Count >= iNumOfFurnitureForLevelFinish)
        {
            //Add code to allow for exit to open here
        }
    }

    //Resets furniture postions should we need to
    public void ResetFurniturePositions()
    {
        foreach(Furniture furniture in ruinedFurniture)
        {
            furniture.transform.position = furniture.fDefaultPosition;
            furniture.bKnockedDown = false;
        }
        ruinedFurniture.Clear();
    }
}
