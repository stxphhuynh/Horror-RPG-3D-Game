using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorScriptController : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("NO ANIMATOR FOUND on this GameObject!");
        }
    }

    void Update()
    {
        bool pressingW = Input.GetKey(KeyCode.W);
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool running = pressingW && shiftHeld;

        // Walking when W is held and NOT running
        animator.SetBool("isWalking", pressingW && !running);

        // Running when W + Shift
        animator.SetBool("RunForward", running);

        // Debug so we can SEE what's happening
        Debug.Log($"W: {pressingW}, Shift: {shiftHeld}, RunForward param: {running}");
    }
}
