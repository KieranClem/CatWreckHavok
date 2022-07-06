using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    [HideInInspector]public bool canFinish = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(canFinish)
        {
            //Insert change scene info here
        }
    }
}
