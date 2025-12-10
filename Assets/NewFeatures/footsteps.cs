using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footsteps : MonoBehaviour
{
    public AudioSource footstepsSound;  // walking
    public AudioSource runningSound;    // running

    void Update()
    {
        bool moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                      Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        bool running = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W);

        if (moving)
        {
            if (running)
            {
                // RUNNING: enable running sound, disable walking
                runningSound.enabled = true;
                footstepsSound.enabled = false;
            }
            else
            {
                // WALKING: enable walking sound, disable running
                footstepsSound.enabled = true;
                runningSound.enabled = false;
            }
        }
        else
        {
            // STOPPED: both off
            footstepsSound.enabled = false;
            runningSound.enabled = false;
        }
    }
}
