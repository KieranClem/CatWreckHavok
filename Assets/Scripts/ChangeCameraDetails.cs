using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChangeCameraDetails : MonoBehaviour
{
    public static ChangeCameraDetails AdjustCamera;
    
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    [Header("Sping Camera Info")]
    public float SpringOrthographicSize;
    public float SpringNearClipPlane;

    [Header("Dash Camera Info")]
    public float DashOrthographicSize;
    public float DashNearClipPlane;

    [Header("Slam Camera Info")]
    public float SlamOrthographicSize;
    public float SlamNearClipPlane;

    private void Awake()
    {
        AdjustCamera = this;
        cinemachineVirtualCamera = this.GetComponent<CinemachineVirtualCamera>();
    }

    public void ChangeCamera(PlayerController player)
    {
        switch(player.currentCharacter)
        {
            case PlayableCharacter.Spring:
                cinemachineVirtualCamera.m_Lens.OrthographicSize = SpringOrthographicSize;
                cinemachineVirtualCamera.m_Lens.NearClipPlane = SpringNearClipPlane;
                break;
            case PlayableCharacter.Dash:
                cinemachineVirtualCamera.m_Lens.OrthographicSize = DashOrthographicSize;
                cinemachineVirtualCamera.m_Lens.NearClipPlane = DashNearClipPlane;
                break;
            case PlayableCharacter.Slam:
                cinemachineVirtualCamera.m_Lens.OrthographicSize = SlamOrthographicSize;
                cinemachineVirtualCamera.m_Lens.NearClipPlane = SlamNearClipPlane;
                break;
        }
    }
}
