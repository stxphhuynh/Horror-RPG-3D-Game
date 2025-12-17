using UnityEngine;

public class InstructionOverlay : MonoBehaviour
{
    [Header("UI")]
    public GameObject instructionPanel;   

    [Header("Game")]
    public WaveSpawner waveSpawner;       // Drag WaveManager here

    private bool isVisible = true;

    void Start()
    {
        // Show instructions and pause the game
        if (instructionPanel != null)
            instructionPanel.SetActive(true);

        Time.timeScale = 0f;  // Pause game while instructions are up
    }

    void Update()
    {
        if (!isVisible) return;

        // Press E to start the game
        if (Input.GetKeyDown(KeyCode.E))
        {
            HideInstructionsAndStart();
        }
    }

    public void HideInstructionsAndStart()
    {
        isVisible = false;

        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        Time.timeScale = 1f;  // Resume game

        if (waveSpawner != null)
            waveSpawner.BeginSpawning();
    }
}
