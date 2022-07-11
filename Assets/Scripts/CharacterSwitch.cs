using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayableCharacter
{
    Dash,
    Slam,
    Spring
}

public class CharacterSwitch : MonoBehaviour
{
    public PlayableCharacter characterToSwitchTo;
}
