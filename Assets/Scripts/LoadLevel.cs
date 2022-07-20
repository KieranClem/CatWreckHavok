using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public string sLevelToLoadName;

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(sLevelToLoadName);
    }
}
