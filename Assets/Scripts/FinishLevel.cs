using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    [HideInInspector]public bool canFinish = false;
    public string SceneToChangeTo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && canFinish)
        {
            //Insert change scene info here
            SceneManager.LoadScene(SceneToChangeTo);

        }
    }
}
