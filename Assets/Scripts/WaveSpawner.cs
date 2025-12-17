using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] mutantWaypoints;

    [Header("Player")]
    public PlayerHealth playerHealth;

    [Header("UI")]
    public TMP_Text roundText;
    public int totalRounds = 5;

    [Header("Enemy Prefabs")]
    public GameObject mutant1Prefab;
    public GameObject mutant2Prefab;
    public GameObject zombiePrefab;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float spawnDelay = 0.3f;

    private int currentRound = 0;
    private bool isSpawning = false;
    private bool hasStarted = false;

    private struct WaveConfig
    {
        public int mutant1Count;
        public int mutant2Count;
        public int zombieCount;

        public WaveConfig(int m1, int m2, int z)
        {
            mutant1Count = m1;
            mutant2Count = m2;
            zombieCount = z;
        }
    }

    private WaveConfig[] waves;

    void Awake()
    {
        waves = new WaveConfig[]
        {
            new WaveConfig(0, 1, 4),
            new WaveConfig(1, 1, 6),
            new WaveConfig(1, 2, 8),
            new WaveConfig(2, 3, 10),
        };
    }

    void Start()
    {
        // Ensure rounds are reset every time the game/scene starts
        currentRound = 0;
        isSpawning = false;
        hasStarted = false;

        
        if (roundText != null)
        {
            roundText.text = string.Empty;
        }
    }

    public void BeginSpawning()
    {
        if (hasStarted) return;

        hasStarted = true;
        StartNextRound();
    }

    void Update()
    {
        if (hasStarted && !isSpawning && NoEnemiesAlive())
        {
            StartNextRound();
        }
    }

    void StartNextRound()
    {
        // Heal player between rounds (but not before round 1)
        if (currentRound > 0 && playerHealth != null)
        {
            playerHealth.Heal(20);
            Debug.Log($"Player healed +20 (now {playerHealth.CurrentHealth})");
        }

        currentRound++;
        UpdateRoundUI();

        if (currentRound >= 1 && currentRound <= 4)
        {
            StartCoroutine(SpawnWave(waves[currentRound - 1]));
        }
        else if (currentRound == 5)
        {
            WaveConfig bossWave = new WaveConfig(5, 4, 14);
            StartCoroutine(SpawnWave(bossWave));
        }
        else
        {
            Debug.Log("All waves complete! You WIN!");
            if (roundText != null)
                roundText.text = "All Rounds Complete!";
        }
    }

    void UpdateRoundUI()
    {
        if (roundText != null)
        {
            roundText.text = $"Round {currentRound}";
        }
    }

    bool NoEnemiesAlive()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
    }

    IEnumerator SpawnWave(WaveConfig wave)
    {
        isSpawning = true;

        List<GameObject> toSpawn = new List<GameObject>();

        for (int i = 0; i < wave.mutant1Count; i++) toSpawn.Add(mutant1Prefab);
        for (int i = 0; i < wave.mutant2Count; i++) toSpawn.Add(mutant2Prefab);
        for (int i = 0; i < wave.zombieCount; i++) toSpawn.Add(zombiePrefab);

        foreach (GameObject enemy in toSpawn)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject obj = Instantiate(enemy, sp.position, sp.rotation);

            EnemyAI ai = obj.GetComponent<EnemyAI>();
            if (ai != null)
                ai.AssignWaypoints(mutantWaypoints);

            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
    }
}
