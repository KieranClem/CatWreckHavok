using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string LevelName;

    public void Loadlevel ()
    {
        SceneManager.LoadScene(LevelName); //RH: type into the string which level the start button should work with
    }
}
