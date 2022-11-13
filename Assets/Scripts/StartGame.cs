using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public string LevelName;
    private PlayerInput inputActions;
    private PlayerInputActions playerInputActions;
    public Text ControlInfo;
    private int PressCount;
    public GameObject BackGround;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.TitleAndEndButtons.Enable();
        if(ControlInfo != null)
        {
            ControlInfo.gameObject.SetActive(false);
        }
        
        PressCount = 0;
    }

    public void GameStart(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (ControlInfo != null)
            {
                if (PressCount == 0)
                {
                    PressCount += 1;
                    BackGround.SetActive(false);
                    ControlInfo.gameObject.SetActive(true);
                }
                else
                {
                    SceneManager.LoadScene(LevelName);
                }
            }
            else
            {
                SceneManager.LoadScene(LevelName);
            }
        }
    }

    public void End(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Application.Quit();
        }
    }

    public void Loadlevel ()
    {    
        if (PressCount == 0)
        {
            PressCount += 1;
            BackGround.SetActive(false);
            ControlInfo.gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(LevelName); //RH: type into the string which level the start button should work with
        }
    }
}
