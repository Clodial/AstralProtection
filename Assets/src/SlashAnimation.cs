using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAnimation : StateMachineBehaviour
{

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Slash", false);
        animator.gameObject.SendMessageUpwards("Slash");
    }
}
