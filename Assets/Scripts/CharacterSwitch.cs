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
    private Animator animator;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        if (characterToSwitchTo == PlayableCharacter.Dash)
        {
            animator.SetBool("IsDash", true);
            animator.SetBool("IsBash", false);
            animator.SetBool("IsSpring", false);
        }
        else if (characterToSwitchTo == PlayableCharacter.Spring)
        {
            animator.SetBool("IsDash", false);
            animator.SetBool("IsBash", false);
            animator.SetBool("IsSpring", true);
        }
        else if(characterToSwitchTo == PlayableCharacter.Slam)
        {
            animator.SetBool("IsDash", false);
            animator.SetBool("IsBash", true);
            animator.SetBool("IsSpring", false);
        }
    }

    public void ChangeAnimation()
    {
        if (characterToSwitchTo == PlayableCharacter.Dash)
        {
            animator.SetBool("IsDash", true);
            animator.SetBool("IsBash", false);
            animator.SetBool("IsSpring", false);
        }
        else if (characterToSwitchTo == PlayableCharacter.Spring)
        {
            animator.SetBool("IsDash", false);
            animator.SetBool("IsBash", false);
            animator.SetBool("IsSpring", true);
        }
        else if (characterToSwitchTo == PlayableCharacter.Slam)
        {
            animator.SetBool("IsDash", false);
            animator.SetBool("IsBash", true);
            animator.SetBool("IsSpring", false);
        }
    }
}
